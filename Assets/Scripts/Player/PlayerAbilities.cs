using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour, IInputExpander
{
    // DASH
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
    [SerializeField] LayerMask whatIsGrapplable;
    [SerializeField] float overshoot = 5f;
    bool isGrappling;

    Player playerScript;
    ActionMap actions;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        dashUI = Instantiate(dashMeterPrefab, GameSettings.instance.GetCanvas()).GetComponent<DashUI>();
    }

    private void Start()
    {
        dashUI.SetDashVisual(maxDashes);
        dashUI.onDashesRecharged += OnDashRecharged;
    }

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        actions.Abilities.GrappleAbility.performed += ctx =>
        {
            if (isGrappling) return;
            
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,out hit, 50f, whatIsGrapplable))
            {
                playerScript.GetMovementScript().Disable();
                isGrappling = true;

                
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(CalculateLaunchVelocity(transform.position, hit.transform.position) * rb.mass, ForceMode.Impulse);
            }
        };
        actions.Abilities.DashAbility.performed += ctx =>
        {
            if (isDashing || dashes >= maxDashes) return;
            isDashing = true;
            dashes++;
            dashUI.SpendDash();
            playerScript.GetMovementScript().Disable();
            CancelInvoke(nameof(DashTimeout));

            // raycast - make sure there are no obstacles in the way
            float newDist = dashDistance;

            Transform body = playerScript.GetMovementScript().GetBody();
            Transform cam = Camera.main.transform;
            Vector3 end;
            
            // calculate end for the raycast
            if (playerScript.GetMovementScript().IsMoving()) end = body.position + playerScript.GetMovementScript().GetInputMoveDirection() * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

            RaycastHit hit;
            if (Physics.Raycast(body.position, (end - body.position).normalized, 
                out hit, dashDistance, whatIsDashObstacle, QueryTriggerInteraction.Ignore))
            {
                newDist = Vector3.Distance(hit.point, body.position) - 0.5f;
            }

            // re-calculate end in case newDist changed
            if (playerScript.GetMovementScript().IsMoving()) end = body.position + playerScript.GetMovementScript().GetInputMoveDirection() * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

            // lerp it
            dashCurve.ClearKeys();
            dashCurve.AddKey(0,0);
            dashCurve.AddKey(newDist / dashSpeed, 1);

            if (currentDashRoutine != null) StopCoroutine(currentDashRoutine);
            currentDashRoutine = StartCoroutine(DashRoutine(end));
        };
        actions.Abilities.ShieldAbility.performed += ctx =>
        {
            // stop moving
            // no dmg
        };
        actions.Abilities.BuffAbility.performed += ctx =>
        {
            // increase damage
            // increase damage resistance
            // last x seconds
        };

        actions.General.DamageSelf.performed += ctx => 
        {
            GetComponent<IDamageable>().ApplyDamage(1f); 
        };
        actions.General.HealSelf.performed += ctx =>
        {
            GetComponent<IDamageable>().ApplyDamage(-1f);
        };
        actions.General.DamageSelf.Enable();
        actions.General.HealSelf.Enable();

        EnableAllAbilities();
    }

    IEnumerator DashRoutine(Vector3 endPos)
    {
        Vector3 startPos = transform.position;
        float time = 0;
        float dashTime = dashCurve.keys[1].time;
        CinemachineFreeLook camera = playerScript.GetCameraControllerScript().GetCameraTransform().GetComponent<CinemachineFreeLook>();
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

    Vector3 CalculateLaunchVelocity(Vector3 startpoint, Vector3 endpoint)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endpoint.y - startpoint.y;
        float h = displacementY + overshoot;
        Vector3 displacementXZ = new Vector3(endpoint.x - startpoint.x, 0f, endpoint.z - startpoint.z);

        Vector3 velocityY = Vector3.up * MathF.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / (MathF.Sqrt(-2 * h / gravity) 
            + MathF.Sqrt(2 * (displacementY - h) / gravity));
        return velocityXZ + velocityY;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (isGrappling)
        {
            isGrappling = false;
            playerScript.GetMovementScript().Enable();
        }
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
