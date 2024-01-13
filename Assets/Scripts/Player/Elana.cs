using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elana : Player
{
    [Space(10), Header("ABILITIES"), Space(10)]
    [Header("Secondary Attack")]
    [SerializeField] float meleeDamage = 1f;
    [SerializeField] Vector2 knockback;
    [SerializeField] float cooldown = 1f;
    [SerializeField] MeleeHitBox mhb;

    [Header("Secondary Attack")]
    [SerializeField] float shootStartDelay = 0.5f;
    [SerializeField] float bulletDamage = 1f;
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] float bulletLifetime = 1f;
    [SerializeField] float bulletCooldown = 0.25f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootOrigin;
    readonly List<GameObject> pooledProjectiles = new List<GameObject>(20);
    bool shooting = false;
    float shootingCooldownTimer;
    float shootStartTimer;

    [Header("Portal")]
    [SerializeField] float portalRange = 30f;
    //[SerializeField] float portalChargeSpeed = 2f;
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject previewPortalPrefab;
    [SerializeField] LayerMask portalPlacableSurfaces;
    int maxUses = 1;
    Transform previewPortal;
    Vector3 portalAPos;
    Vector3 portalBPos;
    Quaternion portalRotation;
    bool usingPortalAbility;

    [Header("Dash (temp)")]
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


    Rigidbody rb;
    Transform body;
    [SerializeField] CinemachineFreeLook freeLookCam;
    [SerializeField] CinemachineFreeLook aimCam;

    internal override void Awake()
    {
        base.Awake();
        body = movementScript.GetBody();
        rb = movementScript.GetRigidbody();

        dashUI = Instantiate(dashMeterPrefab, GameManager.Instance.canvas).GetComponent<DashUI>();
        dashUI.SetDashVisual(maxDashes);
        dashUI.onDashesRecharged += () => { dashes = 0; };

        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectilePrefab).GetComponent<MagicBullet>();
            mb.damage = bulletDamage;
            mb.lifetime = bulletLifetime;
            mb.owner = this;
            pooledProjectiles.Add(mb.gameObject);
        }

        mhb.damage = meleeDamage;
        mhb.knockback = knockback;
    }

    private void Update()
    {
        shootingCooldownTimer += Time.deltaTime;
        if (shooting && (shootStartTimer += Time.deltaTime) > shootStartDelay && shootingCooldownTimer > bulletCooldown)
        {
            ShootMagicBullet();
        }

    }

    void FixedUpdate()
    {
        if (usingPortalAbility)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, StaticUtilities.GetCameraLook(), out hit, portalRange, portalPlacableSurfaces, QueryTriggerInteraction.Ignore))
            {
                previewPortal.position = hit.point + Vector3.up + previewPortal.forward;
                previewPortal.LookAt(transform.position);
            }
            else
            {
                previewPortal.position = transform.position + StaticUtilities.GetCameraDir() * portalRange;
                previewPortal.LookAt(transform.position);
            }
        }
    }

    public override void SetupInputEvents()
    {
        base.SetupInputEvents();
        // Elana's Abilities


        // BASIC
        actions.Abilities.PrimaryAttack.performed += ctx =>
        {
            // melee
            mhb.gameObject.SetActive(true);

        };
        actions.Abilities.SecondaryAttack.started += ctx =>
        {
            shooting = true;
            // aim

            //freeLookCam.gameObject.SetActive(false);
            //aimCam.gameObject.SetActive(true);
        };
        actions.Abilities.SecondaryAttack.canceled += ctx =>
        {
            shooting = false;
            //freeLookCam.gameObject.SetActive(true);
            //aimCam.gameObject.SetActive(false);
            shootStartTimer = 0;
        };


        // Abilities

        // PORTAL ABILITY
        actions.Abilities.First.started += ctx =>
        {
            usingPortalAbility = true;
            previewPortal = Instantiate(previewPortalPrefab).transform;
        };
        actions.Abilities.First.canceled += ctx =>
        {
            usingPortalAbility = false;

            portalRotation = previewPortal.rotation;
            portalAPos = body.position + StaticUtilities.GetCameraDir() * 1.5f;
            portalBPos = previewPortal.position;

            Destroy(previewPortal.gameObject);

            Transform entryPortal = Instantiate(portalPrefab, portalAPos, portalRotation).transform;
            entryPortal.forward = -entryPortal.forward;
            Transform exitPortal = Instantiate(portalPrefab, portalBPos, portalRotation).transform;
            var f = entryPortal.GetComponent<Portal>();
            var s = exitPortal.GetComponent<Portal>();
            f.Init(maxUses, s);
            s.Init(maxUses, f);
        };

        // DASH (TEMPORARY - WILL BE IMPLEMENTED INTO THE PORTAL ABILITY)
        actions.Abilities.Second.started += ctx =>
        {
            if (isDashing || dashes >= maxDashes) return;
            isDashing = true;
            dashes++;
            dashUI.SpendDash();
            CancelInvoke(nameof(DashTimeout));

            // raycast - make sure there are no obstacles in the way
            float newDist = dashDistance;

            Transform body = movementScript.GetBody();
            Transform cam = Camera.main.transform;
            Vector3 end;

            // calculate end for the raycast
            if (movementScript.IsMoving()) end = body.position + movementScript.GetMoveDirection() * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

            RaycastHit hit;
            if (Physics.Raycast(body.position, (end - body.position).normalized,
                out hit, dashDistance, whatIsDashObstacle, QueryTriggerInteraction.Ignore))
            {
                newDist = Vector3.Distance(hit.point, body.position) - 0.5f;
            }

            // re-calculate end in case newDist changed
            if (movementScript.IsMoving()) end = body.position + movementScript.GetMoveDirection() * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

            // lerp it
            dashCurve.ClearKeys();
            dashCurve.AddKey(0, 0);
            dashCurve.AddKey(newDist / dashSpeed, 1);

            if (currentDashRoutine != null) StopCoroutine(currentDashRoutine);
            currentDashRoutine = StartCoroutine(DashRoutine(end));
        };
        actions.Abilities.Second.canceled += ctx => { };

        actions.Abilities.Third.started += ctx => { };
        actions.Abilities.Third.canceled += ctx => { };

        actions.Abilities.Fourth.started += ctx => { };
        actions.Abilities.Fourth.canceled += ctx => { };

        actions.Abilities.Enable();
    }

    IEnumerator DashRoutine(Vector3 endPos)
    {
        movementScript.Disable();
        Vector3 startPos = transform.position;
        float time = 0;
        float dashTime = dashCurve.keys[1].time;
        float startFOV = freeLookCam.m_Lens.FieldOfView;
        float targetFOV = StaticUtilities.defaultFOV + dashFovIncrease;
        while (time < dashTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, dashCurve.Evaluate(time += Time.deltaTime));

            // trying to "lerp" fov from whatever it was to the max
            if (startFOV < targetFOV)
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV + dashFovIntensityCurve.Evaluate(time / dashTime) * dashFovIncrease, startFOV, targetFOV);
            else
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV + dashFovIntensityCurve.Evaluate(time / dashTime) * dashFovIncrease, targetFOV, startFOV);

            yield return null;
        }

        // dash end
        movementScript.Enable();
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
        startFOV = freeLookCam.m_Lens.FieldOfView;
        targetFOV = StaticUtilities.defaultFOV;
        while (time < 1)
        {
            // "lerp" back to default fov
            if (targetFOV < startFOV)
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV - fovRestoreCurve.Evaluate(time) * dashFovIncrease, targetFOV, startFOV);
            else
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV - fovRestoreCurve.Evaluate(time) * dashFovIncrease, startFOV, targetFOV);

            time += Time.deltaTime * fovRestoreSpeed;
            yield return null;
        }
        freeLookCam.m_Lens.FieldOfView = StaticUtilities.defaultFOV;
    }

    void DashTimeout()
    {
        dashes = maxDashes;
        dashUI.RechargeDashes(dashCooldown);
    }

    void ShootMagicBullet()
    {
        shootingCooldownTimer = 0f;
        // start shooting
        foreach (var bullet in pooledProjectiles)
        {
            if (bullet.activeSelf) continue;

            bullet.SetActive(true);
            bullet.transform.position = shootOrigin.position;
            bullet.GetComponent<Rigidbody>().AddForce(StaticUtilities.GetCameraLook() * bulletSpeed + Camera.main.transform.right/2, ForceMode.Impulse);
            break;
        }
    }
}