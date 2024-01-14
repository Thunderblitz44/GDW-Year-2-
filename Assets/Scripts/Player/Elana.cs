using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elana : Player
{
    [Space(10), Header("ABILITIES"), Space(10)]
    [Header("Primary Attack")]
    [SerializeField] float meleeDamage = 1f;
    [SerializeField] Vector2 knockback;
    //[SerializeField] float cooldown = 1f;
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
    [SerializeField] GameObject portalPrefab;
    [SerializeField] LineRenderer portalLink;
    GameObject instantiatedPortal;
    Vector3 recallPos;

    [Header("Dodge")]
    [SerializeField] float dodgeDistance = 5f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] float dodgeCooldown = 1f;
    [SerializeField] AnimationCurve dodgeCurve;
    [SerializeField] AnimationCurve dodgeFovIntensityCurve;
    [SerializeField] float dodgeFovIncrease = 10;
    [SerializeField] AnimationCurve fovRestoreCurve;
    [SerializeField] float fovRestoreSpeed = 5;
    [SerializeField] LayerMask whatIsdodgeObstacle;
    Coroutine currentdodgeRoutine;
    bool isdodgeing = false;


    Rigidbody rb;
    Transform body;
    [SerializeField] CinemachineFreeLook freeLookCam;
    [SerializeField] CinemachineFreeLook aimCam;

    internal override void Awake()
    {
        base.Awake();
        body = movementScript.GetBody();
        rb = movementScript.GetRigidbody();

        abilityHud.onPointRecharged += (i) => 
        { 
            if (i == 0)
            {

            }
            else if (i == 1)
            {

            }
            else if (i == 2)
            {

            }
            else if (i == 3)
            {
                isdodgeing = false;
            }
        };

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

        if (instantiatedPortal)
        {
            if (StaticUtilities.FastDistance(recallPos, transform.position) > (portalRange * portalRange))
            {
                portalLink.enabled = false;
                Destroy(instantiatedPortal);
            }
            else
            {
                portalLink.SetPosition(0, recallPos);
                portalLink.SetPosition(1, transform.position);
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

        };
        actions.Abilities.SecondaryAttack.canceled += ctx =>
        {
            shooting = false;
            shootStartTimer = 0;
        };


        // Abilities

        // PORTAL ABILITY
        actions.Abilities.Portal.performed += ctx =>
        {
            if (instantiatedPortal)
            {
                // lerp to portal
                portalLink.enabled = false;
                dodge(transform.position, recallPos);
                Destroy(instantiatedPortal);
                return;
            }
            
            portalLink.enabled = true;
            recallPos = transform.position;
            instantiatedPortal = Instantiate(portalPrefab, recallPos, Quaternion.LookRotation(StaticUtilities.GetCameraDir(), Vector3.up));
        };

        // Dodge
        actions.Locomotion.Dodge.started += ctx =>
        {
            if (isdodgeing) return;
            abilityHud.SpendPoint(3, dodgeCooldown);

            // raycast - make sure there are no obstacles in the way
            float newDist = dodgeDistance;

            Transform body = movementScript.GetBody();
            Transform cam = Camera.main.transform;
            Vector3 end;

            // calculate end for the raycast
            if (movementScript.IsMoving()) end = body.position + movementScript.GetMoveDirection() * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

            RaycastHit hit;
            if (Physics.Raycast(body.position, (end - body.position).normalized,
                out hit, dodgeDistance, whatIsdodgeObstacle, QueryTriggerInteraction.Ignore))
            {
                newDist = Vector3.Distance(hit.point, body.position) - 0.5f;
            }

            // re-calculate end in case newDist changed
            if (movementScript.IsMoving()) end = body.position + movementScript.GetMoveDirection() * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

            dodge(transform.position, end);
        };

        actions.Abilities.Enable();
    }

    void dodge(Vector3 start, Vector3 end)
    {
        isdodgeing = isInvincible = true;
        float dist = Vector3.Distance(start, end);

        // lerp it
        dodgeCurve.ClearKeys();
        dodgeCurve.AddKey(0, 0);
        dodgeCurve.AddKey(dist / dodgeSpeed, 1);

        if (currentdodgeRoutine != null) StopCoroutine(currentdodgeRoutine);
        currentdodgeRoutine = StartCoroutine(dodgeRoutine(start, end));
    }

    IEnumerator dodgeRoutine(Vector3 startPos, Vector3 endPos)
    {
        movementScript.Disable();
        float time = 0;
        float dodgeTime = dodgeCurve.keys[1].time;
        float startFOV = freeLookCam.m_Lens.FieldOfView;
        float targetFOV = StaticUtilities.defaultFOV + dodgeFovIncrease;
        while (time < dodgeTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, dodgeCurve.Evaluate(time += Time.deltaTime));

            // trying to "lerp" fov from whatever it was to the max
            if (startFOV < targetFOV)
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV + dodgeFovIntensityCurve.Evaluate(time / dodgeTime) * dodgeFovIncrease, startFOV, targetFOV);
            else
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV + dodgeFovIntensityCurve.Evaluate(time / dodgeTime) * dodgeFovIncrease, targetFOV, startFOV);

            yield return null;
        }

        // dodge end
        movementScript.Enable();
        isInvincible = false;

        // restore fov
        time = 0f;
        startFOV = freeLookCam.m_Lens.FieldOfView;
        targetFOV = StaticUtilities.defaultFOV;
        while (time < 1)
        {
            // "lerp" back to default fov
            if (targetFOV < startFOV)
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV - fovRestoreCurve.Evaluate(time) * dodgeFovIncrease, targetFOV, startFOV);
            else
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV - fovRestoreCurve.Evaluate(time) * dodgeFovIncrease, startFOV, targetFOV);

            time += Time.deltaTime * fovRestoreSpeed;
            yield return null;
        }
        freeLookCam.m_Lens.FieldOfView = StaticUtilities.defaultFOV;
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