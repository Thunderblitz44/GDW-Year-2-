using Cinemachine;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerAbilities : NetworkBehaviour, IInputExpander
{
    [Header("DASH"), Space(5f)]
    [SerializeField] float dashDistance = 20f;
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] int maxDashes = 3;
    [SerializeField] float timeLimitBetweenDashes = 1f;
    [SerializeField] float dashCooldown = 5f;
    [SerializeField] AnimationCurve dashCurve;
    [SerializeField] AnimationCurve dashFovIntensityCurve;
    [SerializeField] float dashFovIncrease = 10;
    [SerializeField] AnimationCurve fovRestoreCurve;
    [SerializeField] float fovRestoreSpeed = 5;
    [SerializeField] LayerMask whatIsDashObstacle;
    [SerializeField] GameObject dashMeterPrefab;
    Coroutine currentDashRoutine;
    DashUI dashUI;
    bool isDashing = false;
    int dashes = 0;


    [Header("GRAPPLE"), Space(5f)]
    [SerializeField] float grappleDistance = 50f;
    [SerializeField] float lightWeightMaxMass = 1f;
    [SerializeField] float overshoot = 5f;
    [SerializeField] float playerLandingSpace = 1f;
    [SerializeField] float meetInTheMiddleSpacing = 0.5f;
    [SerializeField] float itemLandingSpace = 1f;
    [SerializeField] float grappleCooldown = 3f;
    [SerializeField] LayerMask whatIsGrapplable;
    [SerializeField] GameObject grappleMeterPrefab;
    bool isGrappling = false;
    bool grappleReady = true;
    GrappleUI grappleUI;
    bool chargeGrapple = false;
    float grappleChargeTime;


    [Header("PORTAL"), Space(5f)]
    [SerializeField] float minTeleportDist = 5f;
    [SerializeField] float maxTeleportDist = 30f;
    [SerializeField] float portalChargeSpeed = 2f;
    [SerializeField] int maxUses = 2;
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject previewPortalPrefab;
    Transform previewPortal;
    Vector3 portalAPos;
    Vector3 portalBPos;
    Quaternion portalRotation;
    bool chargePortals;
    float previewDist;
    Coroutine currentTeleportRoutine;
    bool usingPortalAbility;
    int uses;
    float portalChargeTime;


    [Header("JUMP"), Space(5f)]
    [SerializeField] float minJumpDist = 5f;
    [SerializeField] float maxJumpDist = 15f;
    [SerializeField] float jumpChargeSpeed = 7f;
    [SerializeField] float height = 10f;
    [SerializeField] GameObject falloffMarker;
    Transform jumpFalloff;
    bool chargeJump;
    float jumpChargeTime;
    float falloffDist;


    Player playerScript;
    ActionMap actions;
    Rigidbody rb;
    Transform body;

    private void Start()
    {
        if (!IsOwner) return;

        rb = GetComponent<Rigidbody>();
        body = playerScript.GetMovementScript().GetBody();

        dashUI = Instantiate(dashMeterPrefab, GameSettings.instance.GetCanvas()).GetComponent<DashUI>();
        dashUI.SetDashVisual(maxDashes);
        dashUI.onDashesRecharged += OnDashRecharged;
        
        grappleUI = Instantiate(grappleMeterPrefab, GameSettings.instance.GetCanvas()).GetComponent<GrappleUI>();
        grappleUI.onGrappleRecharged += OnGrappleRecharged;
    }

    private void Update()
    {
        if (chargePortals)
        {
            portalChargeTime += Time.deltaTime * portalChargeSpeed;
            SetPortalDistance(portalChargeTime);
            previewPortal.rotation = body.rotation;
        }
        if (chargeGrapple)
        {
            grappleChargeTime += Time.deltaTime;
        }
        if (chargeJump)
        {
            jumpChargeTime += Time.deltaTime * jumpChargeSpeed;
            SetJumpFalloffPosition(jumpChargeTime);
        }
    }

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        
        actions.Abilities.GrappleAbility.performed += ctx =>
        {
            if (isGrappling || !grappleReady) return;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grappleDistance, whatIsGrapplable))
            {
                grappleUI.SpendPoint();

                Vector3 launchForce = Vector3.zero;
                Transform target = hit.transform;
                Vector3 dir = (target.position - transform.position).normalized;

                // process the grapple target
                if (!hit.rigidbody)
                {
                    // grapple point - we go to it
                    float dist = Vector3.Distance(transform.position, target.position) - playerLandingSpace;
                    Vector3 playerEndPoint = transform.position + dir * dist;
                    launchForce = CalculateLaunchVelocity(transform.position, playerEndPoint, overshoot);
                }
                else if (hit.rigidbody.mass > lightWeightMaxMass)
                {
                    // something heavy, meet in the middle
                    float dist = Vector3.Distance(transform.position, target.position) / 2 - meetInTheMiddleSpacing;
                    Vector3 playerEndPoint = transform.position + dir * dist;
                    Vector3 targetEndPoint = target.position - dir * dist;

                    launchForce = CalculateLaunchVelocity(transform.position, playerEndPoint, overshoot);

                    // tell the server to launch the target
                    LaunchTargetServerRpc(target.GetComponent<NetworkObject>().NetworkObjectId, CalculateLaunchVelocity(target.position, targetEndPoint, overshoot) * hit.rigidbody.mass);
                }
                else
                {
                    // light object, it comes to us
                    float dist = Vector3.Distance(transform.position, target.position) - itemLandingSpace;
                    Vector3 targetEndPoint = target.position - dir * dist;
                    
                    // tell the server to launch the target
                    LaunchTargetServerRpc(target.GetComponent<NetworkObject>().NetworkObjectId, CalculateLaunchVelocity(target.position, targetEndPoint, overshoot) * hit.rigidbody.mass);
                    goto cooldown;
                }

                isGrappling = true;
                playerScript.GetMovementScript().Disable();
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(launchForce * rb.mass, ForceMode.Impulse);

            cooldown:
                grappleReady = false;
                grappleUI.RechargePoint(grappleCooldown);
            }
        };

        actions.Abilities.DashAbility.performed += ctx =>
        {
            if (isDashing || dashes >= maxDashes) return;
            isDashing = true;
            dashes++;
            dashUI.SpendDash();
            CancelInvoke(nameof(DashTimeout));

            // raycast - make sure there are no obstacles in the way
            float newDist = dashDistance;

            Transform body = playerScript.GetMovementScript().GetBody();
            Transform cam = Camera.main.transform;
            Vector3 end;
            
            // calculate end for the raycast
            if (playerScript.GetMovementScript().IsMoving()) end = body.position + playerScript.GetMovementScript().GetMoveDirection() * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

            RaycastHit hit;
            if (Physics.Raycast(body.position, (end - body.position).normalized, 
                out hit, dashDistance, whatIsDashObstacle, QueryTriggerInteraction.Ignore))
            {
                newDist = Vector3.Distance(hit.point, body.position) - 0.5f;
            }

            // re-calculate end in case newDist changed
            if (playerScript.GetMovementScript().IsMoving()) end = body.position + playerScript.GetMovementScript().GetMoveDirection() * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

            // lerp it
            dashCurve.ClearKeys();
            dashCurve.AddKey(0,0);
            dashCurve.AddKey(newDist / dashSpeed, 1);

            if (currentDashRoutine != null) StopCoroutine(currentDashRoutine);
            currentDashRoutine = StartCoroutine(DashRoutine(end));
        };

        actions.Abilities.PortalAbility.started += ctx => 
        {
            if (usingPortalAbility) return;

            usingPortalAbility = true;
            chargePortals = true;

            previewPortal = Instantiate(previewPortalPrefab, transform.position + GetCameraDir() * minTeleportDist, body.rotation).transform;
        };
        actions.Abilities.PortalAbility.canceled += ctx =>
        {
            chargePortals = false;
            portalChargeTime = 0f;

            portalRotation = previewPortal.rotation;
            portalAPos = body.position + body.forward * 1.5f;
            portalBPos = previewPortal.position;

            Destroy(previewPortal.gameObject);

            // set up the portals
            Transform entryPortal = Instantiate(portalPrefab, portalAPos, portalRotation).transform;
            entryPortal.forward = -entryPortal.forward;
            Transform exitPortal = Instantiate(portalPrefab, portalBPos, portalRotation).transform;
            var f = entryPortal.GetComponent<Portal>();
            var s = exitPortal.GetComponent<Portal>();
            f.Init(maxUses,s);
            s.Init(maxUses,f);

            usingPortalAbility = false;
        };

        actions.Abilities.ShieldAbility.performed += ctx =>
        {
            // instantiate an object with a collider - in front of player
            // follow player
            // have a small amount of delay with movement (juice)
            // stay in front of player
            // have a small rotational delay (juice)
            // hold it for x seconds or until released
            // idea: cooldown is the held length
        };
        actions.Abilities.BuffAbility.performed += ctx =>
        {
            // increase damage
            // increase damage resistance
            // last x seconds
        };

        actions.Abilities.JumpAbility.started += ctx =>
        {
            if (chargeJump) return;
            chargeJump = true;

            jumpFalloff = Instantiate(falloffMarker, transform.position, Quaternion.identity).transform;
        };
        actions.Abilities.JumpAbility.canceled += ctx => 
        {
            chargeJump = false;
            jumpChargeTime = 0;
            rb.AddForce(CalculateLaunchVelocity(transform.position, jumpFalloff.position, height) * rb.mass, ForceMode.Impulse);
            Destroy(jumpFalloff.gameObject, 0.5f);
        };


        // for testing
        actions.General.DamageSelf.performed += ctx => 
        {
            if (IsOwner) GetComponent<IDamageable>().ApplyDamage(1f, DamageTypes.physical); 
        };
        actions.General.HealSelf.performed += ctx =>
        {
            if (IsOwner) GetComponent<IDamageable>().ApplyDamage(-1f, DamageTypes.physical);
        };

        // For testing
        actions.General.Attack.performed += ctx =>
        {
            GameObject.Find("TestDummy").GetComponent<IDamageable>().ApplyDamage(1f , DamageTypes.physical);
        };
        actions.CameraControl.Aim.performed += ctx =>
        { 
            GameObject.Find("TestDummy").GetComponent<IDamageable>().ApplyDamage(-1f, DamageTypes.magic);
        };

        actions.General.Attack.Enable();
        actions.General.DamageSelf.Enable();
        actions.General.HealSelf.Enable();


        EnableAllAbilities();
    }

    void SetPortalDistance(float time)
    {
        previewDist = Mathf.Clamp(time + minTeleportDist, minTeleportDist, maxTeleportDist);
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, GetCameraDir()), out hit, previewDist))
        {
            previewPortal.position = new Vector3(hit.point.x, previewPortal.position.y, hit.point.z) - GetCameraDir();
        }
        else
        {
            previewPortal.position = transform.position + GetCameraDir() * previewDist;
        }
    }

    void SetJumpFalloffPosition(float time)
    {
        falloffDist = Mathf.Clamp(time + minJumpDist, minJumpDist, maxJumpDist);
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, GetCameraDir()), out hit, falloffDist))
        {
            jumpFalloff.position = new Vector3(hit.point.x, 0.01f, hit.point.z) - GetCameraDir();
        }
        else
        {
            jumpFalloff.position = new Vector3(transform.position.x, 0.01f, transform.position.z) + GetCameraDir() * falloffDist;
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void LaunchTargetServerRpc(ulong networkObjectId, Vector3 force)
    {
        NetworkObject no = NetworkManager.SpawnManager.SpawnedObjects[networkObjectId];
        no.GetComponent<Rigidbody>().AddForce(force ,ForceMode.Impulse);
    }

    IEnumerator DashRoutine(Vector3 endPos)
    {
        playerScript.GetMovementScript().Disable();
        Vector3 startPos = transform.position;
        float time = 0;
        float dashTime = dashCurve.keys[1].time;
        CinemachineFreeLook camera = playerScript.GetCameraControllerScript().GetFreeLookCamera();
        float startFOV = camera.m_Lens.FieldOfView;
        float targetFOV = GameSettings.instance.defaultFOV + dashFovIncrease;
        while (time < dashTime)
        {
            transform.position = Vector3.Lerp(startPos,endPos, dashCurve.Evaluate(time += Time.deltaTime));

            // trying to "lerp" fov from whatever it was to the max
            if (startFOV < targetFOV)
                camera.m_Lens.FieldOfView = Mathf.Clamp(startFOV + dashFovIntensityCurve.Evaluate(time/dashTime) * dashFovIncrease, startFOV, targetFOV);
            else
                camera.m_Lens.FieldOfView = Mathf.Clamp(startFOV + dashFovIntensityCurve.Evaluate(time / dashTime) * dashFovIncrease, targetFOV, startFOV);

            yield return null;
        }

        // dash end
        playerScript.GetMovementScript().Enable();
        isDashing = false;

        // cooldown
        if (dashes >= maxDashes)
        {
            dashUI.RechargeDashes(dashCooldown);
        }
        else
        {
            Invoke(nameof(DashTimeout), timeLimitBetweenDashes);
        }

        // restore fov
        time = 0f;
        startFOV = camera.m_Lens.FieldOfView;
        targetFOV = GameSettings.instance.defaultFOV;
        while (time < 1)
        {
            // "lerp" back to default fov
            if (targetFOV < startFOV)
                camera.m_Lens.FieldOfView = Mathf.Clamp(startFOV - fovRestoreCurve.Evaluate(time) * dashFovIncrease, targetFOV, startFOV);
            else
                camera.m_Lens.FieldOfView = Mathf.Clamp(startFOV - fovRestoreCurve.Evaluate(time) * dashFovIncrease, startFOV, targetFOV);

            time += Time.deltaTime * fovRestoreSpeed;
            yield return null;
        }
        camera.m_Lens.FieldOfView = GameSettings.instance.defaultFOV;
    }

    void OnDashRecharged()
    {
        dashes = 0;
    }

    void DashTimeout()
    {
        dashes = maxDashes;
        dashUI.RechargeDashes(dashCooldown);
    }

    Vector3 CalculateLaunchVelocity(Vector3 startpoint, Vector3 endpoint, float overshoot)
    {
        float gravity = Physics.gravity.y;
        float displacementY = Math.Abs(endpoint.y - startpoint.y);
        float h = displacementY + overshoot;

        Vector3 displacementXZ = new Vector3(endpoint.x - startpoint.x, 0f, endpoint.z - startpoint.z);
        Vector3 velocityY = Vector3.up * MathF.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / (MathF.Sqrt(-2 * h / gravity) 
            + MathF.Sqrt(2 * (displacementY - h) / gravity));
        return velocityXZ + velocityY;
    }

    private void OnGrappleRecharged()
    {
        grappleReady = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGrappling)
        {
            isGrappling = false;
            playerScript.GetMovementScript().Enable();
        }
    }

    Vector3 GetCameraDir()
    {
        return new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
    }

    public void EnableAllAbilities() => actions.Abilities.Enable();
    public void DisableAllAbilities() => actions.Abilities.Disable();

    public void EnableGrappleAbility() => actions.Abilities.GrappleAbility.Enable();
    public void DisableGrappleAbility() => actions.Abilities.GrappleAbility.Disable();

    public void EnableDashAbility() => actions.Abilities.DashAbility.Enable();
    public void DisableDashAbility() => actions.Abilities.DashAbility.Disable();

    public void EnableShieldAbility() => actions.Abilities.ShieldAbility.Enable();
    public void DisableShieldAbility() => actions.Abilities.ShieldAbility.Disable();

    public void EnableBuffAbility() => actions.Abilities.BuffAbility.Enable();
    public void DisableBuffAbility() => actions.Abilities.BuffAbility.Disable();
}
