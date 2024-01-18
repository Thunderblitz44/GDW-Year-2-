using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Elana : Player
{
    [Space(10), Header("ABILITIES"), Space(10)]
    [Header("Primary Attack")]
    [SerializeField] float meleeDamage = 1f;
    [SerializeField] Vector2 knockback;
    //[SerializeField] float cooldown = 1f;
    [SerializeField] MeleeHitBox mhb;
   public static bool isPrimaryAttacking;
    
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
    [SerializeField] float teleportSpeed = 40f;
    [SerializeField] float portalCoolown = 1f;
    [SerializeField] GameObject recallPointIndicatorPrefab;
    //[SerializeField] GameObject portalPrefab;
    [SerializeField] LineRenderer portalLink;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    GameObject instantiatedPortal;
    Vector3 recallPos;
    bool canPortal = true;
    const int portalId = 2;

    [Header("Fire Tornado")]
    [SerializeField] float maxRange = 15f;
    [SerializeField] float burnDamage = 1f;
    [SerializeField] float burnTime = 3f;
    [SerializeField] float rainTime = 5f;
    [SerializeField] float tornadoTime = 5f;
    [SerializeField] float tornadoDamageMultiplier = 2f;
    [SerializeField] float tornadoForce = 2f;
    [SerializeField] float tornadoCooldown = 2f;
    [SerializeField] GameObject aoeIndicatorPrefab;
    [SerializeField] GameObject abilityPrefab;
    Transform aoeIndicator;
    GameObject fireTornado;
    bool aimingFireTornado = false;
    bool canUseFireTornado = true;
    bool invalidPlacement = false;
    const int fireTornadoId = 1;

    [Header("Dodge")]
    [SerializeField] float dodgeDistance = 5f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] float dodgeCooldown = 1f;
    [SerializeField] AnimationCurve dodgeCurve;
    [SerializeField] AnimationCurve dodgeFovIntensityCurve;
    [SerializeField] float dodgeFovIncrease = 10;
    [SerializeField] AnimationCurve fovRestoreCurve;
    [SerializeField] float fovRestoreSpeed = 5;
    [SerializeField] LayerMask whatIsDodgeObstacle;
    Coroutine currentdodgeRoutine;
    bool isDodgeing = false;
    bool canDodge = true;
    const int dodgeId = 3;

    //Rigidbody rb;
    //Transform body;

    internal override void Awake()
    {
        base.Awake();
        //body = movementScript.GetBody();
        //rb = GetComponent<Rigidbody>();

        abilityHud.onPointRecharged += (i) => 
        { 
            if (i == 0)
            {

            }
            else if (i == fireTornadoId)
            {
                canUseFireTornado = true;
            }
            else if (i == portalId)
            {
                canPortal = true;
            }
            else if (i == dodgeId)
            {
                canDodge = true;
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
                abilityHud.SpendPoint(portalId, portalCoolown);
            }
            else
            {
                portalLink.SetPosition(0, recallPos);
                portalLink.SetPosition(1, transform.position);
                instantiatedPortal.transform.rotation = Quaternion.LookRotation(StaticUtilities.GetCameraDir(),Vector3.up);
            }
        }

        if (aimingFireTornado)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, StaticUtilities.GetCameraLook(), out hit))
            {
                if (Vector3.Angle(Vector3.up, hit.normal) < 45)
                {
                    if (!aoeIndicator.gameObject.activeSelf) aoeIndicator.gameObject.SetActive(true);
                    aoeIndicator.position = hit.point;
                    invalidPlacement = false;
                }
                else
                {
                    // block
                    if (aoeIndicator.gameObject.activeSelf) aoeIndicator.gameObject.SetActive(false);
                    invalidPlacement = true;
                }
            }
        }
    }

    public override void SetupInputEvents()
    {
        base.SetupInputEvents();
        // Elana's Abilities


        // BASIC
        actions.Abilities.PrimaryAttack.started += ctx =>
        {
            // melee
            //mhb.gameObject.SetActive(true);
            
            
            //Animate player/wolf attacking in sync
            isPrimaryAttacking = true;
        };
        actions.Abilities.PrimaryAttack.canceled += ctx =>
        {
          

            isPrimaryAttacking = false;
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
            if (!canPortal) return;

            if (instantiatedPortal)
            {
                // lerp to portal
                canPortal = false;
                //StartCoroutine(PortalRoutine());
                meshRenderer.enabled = false;
                Dodge(transform.position, recallPos, teleportSpeed);
                return;
            }
            
            portalLink.enabled = true;
            recallPos = transform.position;
            instantiatedPortal = Instantiate(recallPointIndicatorPrefab, recallPos, Quaternion.LookRotation(StaticUtilities.GetCameraDir(), Vector3.up));
        };

        // Dodge
        actions.Locomotion.Dodge.started += ctx =>
        {
            if (isDodgeing || !canDodge) return;
            canDodge = false;
            abilityHud.SpendPoint(dodgeId, dodgeCooldown);

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
                out hit, dodgeDistance, whatIsDodgeObstacle, QueryTriggerInteraction.Ignore))
            {
                newDist = Vector3.Distance(hit.point, body.position) - 0.5f;
            }

            // re-calculate end in case newDist changed
            if (movementScript.IsMoving()) end = body.position + movementScript.GetMoveDirection() * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

            Dodge(transform.position, end, dodgeSpeed);
        };

        actions.Abilities.FireTornado.started += ctx =>
        {
            if (!canUseFireTornado) return;

            aimingFireTornado = true;
            aoeIndicator = Instantiate(aoeIndicatorPrefab).transform;
        };
        actions.Abilities.FireTornado.canceled += ctx =>
        {
            if (!canUseFireTornado) return;

            aimingFireTornado = false;
            if (invalidPlacement)
            {
                Destroy(aoeIndicator.gameObject);
                return;
            }

            // cast it
            canUseFireTornado = false;
            abilityHud.SpendPoint(fireTornadoId, tornadoTime + tornadoCooldown);
            fireTornado = Instantiate(abilityPrefab, aoeIndicator.position, Quaternion.identity);
            Invoke(nameof(EndTornado), tornadoTime);
        };

        actions.Abilities.Enable();
    }

    void Dodge(Vector3 start, Vector3 end, float speed)
    {
        isDodgeing = isInvincible = true;
        float dist = Vector3.Distance(start, end);

        // lerp it
        dodgeCurve.ClearKeys();
        dodgeCurve.AddKey(0, 0);
        dodgeCurve.AddKey(dist / speed, 1);

        if (currentdodgeRoutine != null) StopCoroutine(currentdodgeRoutine);
        currentdodgeRoutine = StartCoroutine(DodgeRoutine(start, end));
    }

    IEnumerator DodgeRoutine(Vector3 startPos, Vector3 endPos)
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

        OnDodgeEnded();

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

    void OnDodgeEnded()
    {
        // dodge end
        movementScript.Enable();
        isInvincible = false;
        isDodgeing = false;

        if (canPortal) return;

        portalLink.enabled = false;
        Destroy(instantiatedPortal);
        abilityHud.SpendPoint(portalId, portalCoolown);
        meshRenderer.enabled = true;
        //foreach (var portal in StaticUtilities.elanaPortals) portal.Die();
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

    void EndTornado()
    {
        fireTornado.GetComponent<VisualEffect>().Stop();
        Invoke(nameof(KillTornado), tornadoCooldown > 0.5f ? tornadoCooldown - 0.5f : 0);
    }

    void KillTornado()
    {
        Destroy(fireTornado);
        Destroy(aoeIndicator.gameObject);
    }

    /*IEnumerator PortalRoutine()
    {
        movementScript.Disable();

        // play animation
        yield return new WaitForSeconds(0.25f);
        Vector3 p1SpawnPos;
        Quaternion p1SpawnRot;
        Debug.Log(rb.velocity);
        if (rb.velocity != Vector3.zero)
        {
            Vector3 normVel = rb.velocity.normalized;
            p1SpawnPos = body.position + normVel;
            p1SpawnRot = Quaternion.LookRotation(body.position - normVel, Vector3.up);
        }
        else
        {
            p1SpawnPos = body.position + transform.forward;
            p1SpawnRot = Quaternion.LookRotation(StaticUtilities.GetCameraDir(), Vector3.up);
        }
        StaticUtilities.elanaPortals[0] = Instantiate(portalPrefab, p1SpawnPos, p1SpawnRot).GetComponent<Portal>();
        StaticUtilities.elanaPortals[1] = Instantiate(portalPrefab, instantiatedPortal.transform.position, instantiatedPortal.transform.rotation).GetComponent<Portal>();
        Destroy(instantiatedPortal);

        // HERE - MAKING THE FIRST PORTAL FACE THE PLAYER WHEN IT SPAWNS

        yield return new WaitForSeconds(0.25f);

        Dodge(transform.position, recallPos, teleportSpeed);
    }*/
}