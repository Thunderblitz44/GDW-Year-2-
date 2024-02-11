using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Elana : Player
{
    [Space(10), Header("ABILITIES"), Space(10)]
    [Header("Primary Attack")]
    [SerializeField] int meleeDamage = 1;
    [SerializeField] Vector2 knockback;
    //[SerializeField] float cooldown = 1f;
    [SerializeField] MeleeHitBox mhb;
    [SerializeField] private SpiritWolfAnimator spiritWolfAnimator;
    
    [Header("Secondary Attack")]
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] float shootStartDelay = 0.5f;
    [SerializeField] float bulletCooldown = 0.25f;
    [SerializeField] Transform shootOrigin;
    readonly List<GameObject> pooledProjectiles = new(20);
    bool shooting = false;
    float shootingCooldownTimer;
    float shootStartTimer;

    [Header("Portal")]
    [SerializeField] float portalRange = 30f;
    [SerializeField] float teleportSpeed = 40f;
    [SerializeField] float portalCoolown = 1f;
    [SerializeField] GameObject recallPointIndicatorPrefab;
    [SerializeField] LineRenderer portalLink;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    GameObject instantiatedPortal;
    Vector3 recallPos;
    bool canPortal = true;
    const int portalId = 2;
   
    [Header("Fire Tornado")]
    [SerializeField] float maxRange = 15f;
    [SerializeField] int tornadoDamage = 1;
    [SerializeField] float burnTime = 3f;
    [SerializeField] float tornadoTime = 5f;
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
    [SerializeField] AnimationCurve dodgeCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [SerializeField] AnimationCurve dodgeFovIntensityCurve;
    [SerializeField] float dodgeFovIncrease = 10;
    [SerializeField] AnimationCurve fovRestoreCurve;
    [SerializeField] float fovRestoreSpeed = 5;
    [SerializeField] LayerMask whatIsDodgeObstacle;
    Coroutine currentdodgeRoutine;
    bool isDodgeing = false;
    bool canDodge = true;
    const int dodgeId = 3;
    [SerializeField] TrailScript TrailScript;
    
    [Header("Other")]
    [SerializeField] Animator specialAnimator;
    //animator to control portal and potentially other interactions between players/spirit
    public float recallDelay = 2f;
    //delay of recall
   
    //for determining the difference between the portal and dodge as they both call the same method
    protected override void Awake()
    {
        base.Awake();

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

        projectile.owner = this;
        projectile.CheckPrefab();
        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectile.prefab).GetComponent<MagicBullet>();
            mb.Initialize(projectile);
            pooledProjectiles.Add(mb.gameObject);
        }

        mhb.damage = meleeDamage;
        mhb.knockback = knockback;

        LevelManager.Instance.onEncounterStart += CancelRecallAbility;
    }

    void Update()
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
            if (Physics.Raycast(Camera.main.ScreenPointToRay(StaticUtilities.GetCenterOfScreen()), out hit, maxRange, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
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
            spiritWolfAnimator.PrimaryAttack();
        };
        actions.Abilities.PrimaryAttack.canceled += ctx =>
        {
          spiritWolfAnimator.EndAttack();

           
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
               
                specialAnimator.SetTrigger("Recall");
                specialAnimator.SetBool("Underground", true);
                canPortal = false;
                meshRenderer.enabled = false;
                TrailScript.isTrailActive2 = true;
                StartCoroutine(DelayedDodge(transform.position, recallPos, teleportSpeed, 1f));
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

            Transform body = MovementScript.Body;
            Transform cam = Camera.main.transform;
            Vector3 end;

            // calculate end for the raycast
            if (MovementScript.IsMoving) end = body.position + MovementScript.MoveDirection * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

            RaycastHit hit;
            if (Physics.Raycast(body.position, (end - body.position).normalized,
                out hit, dodgeDistance, whatIsDodgeObstacle, QueryTriggerInteraction.Ignore))
            {
                newDist = Vector3.Distance(hit.point, body.position) - 0.5f;
            }

            // re-calculate end in case newDist changed
            if (MovementScript.IsMoving) end = body.position + MovementScript.MoveDirection * newDist;
            else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;
          
                TrailScript.isTrailActive = true;
          
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
            fireTornado.GetComponent<FireTornado>().BurnTime = burnTime;
            fireTornado.GetComponent<FireTornado>().Damage = tornadoDamage;
            Invoke(nameof(EndTornado), tornadoTime);
        };

        actions.Abilities.Enable();
    }

    private IEnumerator DelayedDodge(Vector3 startPosition, Vector3 targetPosition, float speed, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Call Dodge after the delay
        Dodge(startPosition, targetPosition, speed);
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
        MovementScript.DisableLocomotion();
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
        MovementScript.EnableLocomotion();
        isInvincible = false;
        isDodgeing = false;
      
        if (canPortal) return;
    
        portalLink.enabled = false;
        Destroy(instantiatedPortal);
        abilityHud.SpendPoint(portalId, portalCoolown);
        meshRenderer.enabled = true;
        specialAnimator.SetBool("Underground", false);
        TrailScript.isTrailActive = false;
        TrailScript.isTrailActive2 = false;
    }

    void ShootMagicBullet()
    {
        shootingCooldownTimer = 0f;
        // start shooting
        Vector3 force;
        RaycastHit hit;
        Ray camLook = Camera.main.ScreenPointToRay(StaticUtilities.GetCenterOfScreen());
        if (Physics.Raycast(camLook, out hit, 100f, whatIsDodgeObstacle, QueryTriggerInteraction.Ignore))
        {
            force = (hit.point - shootOrigin.position).normalized * projectile.speed;
        }
        else
        {
            force = camLook.direction * projectile.speed;
        }
        StaticUtilities.ShootProjectile(pooledProjectiles,shootOrigin.position, force);
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

    void CancelRecallAbility()
    {
        if (!instantiatedPortal) return;

        portalLink.enabled = false;
        Destroy(instantiatedPortal);
        abilityHud.SpendPoint(portalId, portalCoolown);
    }
}