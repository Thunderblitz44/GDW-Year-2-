using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Elana : Player
{
    [Space(10), Header("ABILITIES"), Space(10)]
    [Header("Primary Attack")]
    [SerializeField] float range = 4f;
    [SerializeField] float wolfLerpSpeed = 1;
    [SerializeField] float lockonRadiusOverride = 200f;
    SpiritWolfAnimator spiritWolfAnimator;
    Transform spiritWolf;
    bool melee;
    bool lerpWolf;
    [SerializeField] LayerMask enemyLayer;
    Vector3 wolfLerpStart;
    Vector3 wolfLerpEnd;
    Quaternion wolfRotStart;
    Quaternion wolfRotEnd;
    float wolfLerpTime;
    
    [Header("Secondary Attack")]
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] float shootStartDelay = 0.5f;
    [SerializeField] float bulletCooldown = 0.25f;
    [SerializeField] Transform shootOrigin;
    readonly List<GameObject> pooledProjectiles = new(10);
    bool shooting = false;
    float shootingCooldownTimer;
    float shootStartTimer;

    [Header("Portal")]
    [SerializeField] float portalRange = 30f;
    [SerializeField] float teleportSpeed = 5f;
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
    Pheonix pheonix;
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
    [SerializeField] AnimationCurve dodgeFovIntensityCurve;
    [SerializeField] float dodgeFovIncrease = 10;
    [SerializeField] AnimationCurve fovRestoreCurve;
    //[SerializeField] float fovRestoreSpeed = 5;
    [SerializeField] LayerMask whatIsDodgeObstacle;
    TrailScript TrailScript;
    Coroutine currentdodgeRoutine;
    bool isDodgeing = false;
    bool canDodge = true;
    const int dodgeId = 3;
    
    [Header("Other")]
    [SerializeField] Animator specialAnimator;
   
   
    [SerializeField] public Animator DragonflyAnimator;
    //animator to control portal and potentially other interactions between players/spirit
    public float recallDelay = 2f;
private WindBurst WindBurstRef;
    //delay of recall
   [SerializeField] PlayerAnimator PlayerAnimator;
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
        
        projectile.CheckPrefab();
        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectile.prefab).GetComponent<MagicBullet>();
            mb.Initialize(projectile, this);
            pooledProjectiles.Add(mb.gameObject);
        }

        LevelManager.Instance.onEncounterStart += CancelRecallAbility;

        spiritWolfAnimator = GetComponentInChildren<SpiritWolfAnimator>();
        spiritWolf = spiritWolfAnimator.transform;
        pheonix = GetComponentInChildren<Pheonix>();
        TrailScript = GetComponentInChildren<TrailScript>();
        WindBurstRef = GetComponentInChildren<WindBurst>();
    }

    protected override void Update()
    {
        base.Update();

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
                //instantiatedPortal.transform.rotation = Quaternion.LookRotation(StaticUtilities.GetCameraDir(),Vector3.up);
            }
        }

        if (aimingFireTornado)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(StaticUtilities.GetCenterOfScreen()), out RaycastHit hit, maxRange, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
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

        if (melee) 
        {
            if (wolfLerpTime < 1)
            {
                if (lockonTarget)
                {
                    wolfLerpEnd = FindSpiritWolfAttackPos();
                    wolfRotEnd = Quaternion.LookRotation(wolfLerpEnd - wolfLerpStart);
                }
                else
                {
                    wolfLerpEnd = GetSpiritWolfPassivePos();
                    wolfRotEnd = GetSpiritWolfPassiveRot();
                }

                spiritWolf.SetPositionAndRotation(Vector3.Lerp(wolfLerpStart, wolfLerpEnd, StaticUtilities.easeCurve01.Evaluate(wolfLerpTime)),
                    Quaternion.Slerp(wolfRotStart, wolfRotEnd, StaticUtilities.easeCurve01.Evaluate(wolfLerpTime * 2f)));
            }
            else if (lockonTarget)
            {
                spiritWolf.position = FindSpiritWolfAttackPos();
                spiritWolf.LookAt(lockonTarget);
            }
            else
            {
                spiritWolf.position = GetSpiritWolfPassivePos();
                spiritWolf.rotation = GetSpiritWolfPassiveRot();
                // default pos
            }
            if (lerpWolf) wolfLerpTime += Time.deltaTime * wolfLerpSpeed;
        }
    }

    public override void SetupInputEvents()
    {
        base.SetupInputEvents();
        // Elana's Abilities


        // BASIC
        actions.Abilities.SecondaryAttack.started += ctx =>
        {
            // melee
            melee = true;
            lerpWolf = true;
            wolfLerpTime = 0;
            wolfLerpStart = GetSpiritWolfPassivePos();
            wolfRotStart = GetSpiritWolfPassiveRot();

            autoLockOverride = true;
            autoLockRadiusOverride = lockonRadiusOverride;
            autoLockRangeOverride = range;
            PlayerAnimator.IsUsingWolf();
            //Animate player/wolf attacking in sync
            spiritWolfAnimator.PrimaryAttack();
          //  FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Wolf Swipe", gameObject);
        };
        actions.Abilities.SecondaryAttack.canceled += ctx =>
        {
            spiritWolfAnimator.EndAttack();
            wolfLerpTime = 1;
            spiritWolf.position = GetSpiritWolfPassivePos();
            lerpWolf = false;
            PlayerAnimator.IsNotUsingWolf();
            melee = false;
            autoLockOverride = false;
        };
        actions.Abilities.PrimaryAttack.started += ctx =>
        {
            shooting = true;
            // aim
            DragonflyAnimator.SetBool("IsShooting", true);
            PlayerAnimator.IsUsingDragonFly();
        };
        actions.Abilities.PrimaryAttack.canceled += ctx =>
        {
            shooting = false;
            shootStartTimer = 0;
            DragonflyAnimator.SetBool("IsShooting", false);
            PlayerAnimator.IsNotUsingDragonFly();
        };


        // Abilities

        // PORTAL ABILITY
        actions.Abilities.Portal.performed += ctx =>
        {
            if (!canPortal ) return;
         
            if (instantiatedPortal )
            {
                TrailScript.OnPortalEvent();
                specialAnimator.SetTrigger("Recall");
                specialAnimator.SetBool("Underground", true);
                canPortal = false;
                meshRenderer.enabled = false;
                TrailScript.isTrailActive2 = true;
               
                StartCoroutine(DelayedDodge(transform.position, recallPos, teleportSpeed, 0f));
                      return;
                       
            }
        
            portalLink.enabled = true;
            recallPos = transform.position;
            if (MovementScript.IsGrounded)
            {
                instantiatedPortal = Instantiate(recallPointIndicatorPrefab, recallPos, Quaternion.LookRotation(StaticUtilities.GetCameraDir(), Vector3.up));
            }
          
        };
      
        // Dodge
        actions.Locomotion.Dodge.started += ctx =>
        {
           
            if (isDodgeing || !canDodge) return;
            canDodge = false;
            abilityHud.SpendPoint(dodgeId, dodgeCooldown);
            TrailScript.OnDodge();
            // raycast - make sure there are no obstacles in the way
            float newDist = dodgeDistance;

            Transform body = MovementScript.Body;
            Transform cam = Camera.main.transform;
            Vector3 end;

            // calculate end for the raycast
            if (MovementScript.IsMoving) end = body.position + MovementScript.MoveDirection * newDist;
            else end = body.position + StaticUtilities.GetCameraDir() * newDist;

            RaycastHit hit;
            if (Physics.Raycast(body.position, (end - body.position).normalized,
                out hit, dodgeDistance, whatIsDodgeObstacle, QueryTriggerInteraction.Ignore))
            {
                newDist = Vector3.Distance(hit.point, body.position) - 0.5f;
            }

            // re-calculate end in case newDist changed
            if (MovementScript.IsMoving) end = body.position + MovementScript.MoveDirection * newDist;
            else end = body.position + StaticUtilities.GetCameraDir() * newDist;
          
                TrailScript.isTrailActive = true;
          
            Dodge(transform.position, end, dodgeSpeed);
        };

        actions.Abilities.FireTornado.started += ctx =>
        {
            
            if (!canUseFireTornado) return;
            pheonix.CastAttack();
           
            aimingFireTornado = true;
            aoeIndicator = Instantiate(aoeIndicatorPrefab).transform;
        };
        actions.Abilities.FireTornado.canceled += ctx =>
        {
        Invoke("DelayPheonixDissapear", 4f);
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

    void DelayPheonixDissapear()
    {
        pheonix.EndAttack();
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
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Wave Dash", gameObject);

        // lerp it

        if (currentdodgeRoutine != null) StopCoroutine(currentdodgeRoutine);
        currentdodgeRoutine = StartCoroutine(DodgeRoutine(start, end, speed));
    }

    IEnumerator DodgeRoutine(Vector3 startPos, Vector3 endPos, float speed)
    {
        MovementScript.DisableLocomotion();
        
        float dist = Vector3.Distance(startPos, endPos);
        float startFOV = freeLookCam.m_Lens.FieldOfView;
        float targetFOV = StaticUtilities.defaultFOV + dodgeFovIncrease;

        for (float t = 0; t < dist; t += Time.deltaTime * speed) 
        {
            transform.position = Vector3.Lerp(startPos, endPos, StaticUtilities.easeCurve01.Evaluate(t/dist));

            // trying to "lerp" fov from whatever it was to the max
            if (startFOV < targetFOV)
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV + dodgeFovIntensityCurve.Evaluate(t) * dodgeFovIncrease, startFOV, targetFOV);
            else
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV + dodgeFovIntensityCurve.Evaluate(t) * dodgeFovIncrease, targetFOV, startFOV);

            yield return null;
        }

        // dodge end
        MovementScript.EnableLocomotion();
        MovementScript.Rb.velocity = Vector3.zero;
        isInvincible = false;
        isDodgeing = false;
        MovementScript.Rb.velocity = StaticUtilities.HorizontalizeVector(MovementScript.Rb.velocity);

        if (!canPortal)
        {
            portalLink.enabled = false;
            Destroy(instantiatedPortal);
            abilityHud.SpendPoint(portalId, portalCoolown);
            meshRenderer.enabled = true;
            specialAnimator.SetBool("Underground", false); 
            WindBurstRef.Burst();
            TrailScript.isTrailActive = false;
            TrailScript.isTrailActive2 = false;
        }

        // restore fov
        startFOV = freeLookCam.m_Lens.FieldOfView;
        targetFOV = StaticUtilities.defaultFOV;
        for (float t = 0; t < 1; t += Time.deltaTime * dodgeSpeed)
        {
            // "lerp" back to default fov
            if (targetFOV < startFOV)
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV - fovRestoreCurve.Evaluate(t) * dodgeFovIncrease, targetFOV, startFOV);
            else
                freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(startFOV - fovRestoreCurve.Evaluate(t) * dodgeFovIncrease, startFOV, targetFOV);

            yield return null;
        }
        freeLookCam.m_Lens.FieldOfView = StaticUtilities.defaultFOV;
        currentdodgeRoutine = null;
    }

    void ShootMagicBullet()
    {
        shootingCooldownTimer = 0f;
        // start shooting
        Vector3 force;

        if (autoLock && lockonTarget)
        {
            force = (lockonTarget.position - shootOrigin.position).normalized * projectile.speed;
            StaticUtilities.ShootProjectile(pooledProjectiles, shootOrigin.position, force);
        }
        else
        {
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

    protected override void OnLockonTargetChanged()
    {
        if (melee)
        {
            wolfLerpStart = spiritWolf.position;
            wolfRotStart = spiritWolf.rotation;
            if (lockonTarget)
            {
                wolfLerpEnd = FindSpiritWolfAttackPos();
                wolfRotEnd = GetSpiritWolfPassiveRot();
            }
            else
            {
                wolfLerpEnd = GetSpiritWolfPassivePos();
                wolfRotEnd = Quaternion.LookRotation(MovementScript.Body.forward);
            }
            wolfLerpTime = 0;
        }
    }

    Vector3 GetSpiritWolfPassivePos()
    {
        return MovementScript.Body.position + MovementScript.Body.right + MovementScript.Body.up * 0.25f;
    }

    Quaternion GetSpiritWolfPassiveRot()
    {
        return Quaternion.LookRotation(MovementScript.Body.forward);
    }

    Vector3 FindSpiritWolfAttackPos()
    {
        if (NavMesh.SamplePosition(lockonTarget.position + 0.5f * MovementScript.Body.right - 0.5f * MovementScript.Body.forward, out NavMeshHit hit2, range, NavMesh.AllAreas))
        {
            return StaticUtilities.BuildVector(hit2.position.x, lockonTarget.position.y, hit2.position.z);
        }
        else
        {
            return lockonTarget.position - MovementScript.Body.forward;
        }
    }

    public bool IsAttacking()
    {
        return shooting || melee || aimingFireTornado;
    }
}