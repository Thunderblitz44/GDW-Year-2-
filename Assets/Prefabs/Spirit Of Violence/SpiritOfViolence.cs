using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class SpiritOfViolence : Enemy, IBossCommands
{
    //* PART 2 - ELANA DOPPLEGANGER - *//
    [Header("Behaviour")]
    [SerializeField] int maxAttacksAtOnce = 2;
    [SerializeField] float startAttackingDelay = 2f;
    [SerializeField] float maxViewAngle = 120;
    [SerializeField] float aggressiveChaseDist = 10f;
    bool canSeePlayer;
    int attacksBeingUsed;
    bool battleStarted = false;
    bool pauseAttack;
    Elana player;

    [Header("attacks")]
    [SerializeField] bool enableMelee;
    [SerializeField] bool enableRange;
    [SerializeField] bool enableTornado;
    [SerializeField] bool enableDash;

    [Header("Melee Attack")]
    [SerializeField] float meleeRange = 6f;
    [SerializeField] float wolfLerpSpeed = 1;
    [SerializeField] float minMeleeAttackTime = 1;
    [SerializeField] float maxMeleeAttackTime = 5;
    [SerializeField] float minMeleeCooldown = 2;
    [SerializeField] float maxMeleeCooldown = 5;
    [SerializeField] LayerMask playerLayer;
    SpiritWolfAnimator spiritWolfAnimator;
    Transform spiritWolf;
    float playerDistance = 1000;
    bool melee;
    bool canUseMelee = true;

    [Header("Ranged Attack")]
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] float shootStartDelay = 0.5f;
    [SerializeField] float bulletCooldown = 0.25f;
    [SerializeField] Transform shootOrigin;
    [SerializeField] int minBurst = 10;
    [SerializeField] int maxBurst = 30;
    [SerializeField] public Animator dragonflyAnimator;
    [SerializeField] float minRangedCooldown = 2f;
    [SerializeField] float maxRangedCooldown = 6f;
    readonly List<GameObject> pooledProjectiles = new(8);
    bool shooting;
    bool canUseRanged = true;

    [Header("Dodge")]
    [SerializeField] float dodgeDistance = 6f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] float minDodgeCooldown = 1f;
    [SerializeField] float maxDodgeCooldown = 5f;
    [SerializeField] float dodgeAwayDelay = 1.5f;
    [SerializeField] LayerMask whatIsDodgeObstacle;
    bool isDodgeing = false;
    float dodgeAwayTimer;

    [Header("Fire Tornado")]
    [SerializeField] float maxRange = 15f;
    [SerializeField] int tornadoDamage = 1;
    [SerializeField] float burnTime = 3f;
    [SerializeField] float tornadoTime = 5f;
    [SerializeField] float minTornadoCooldown = 2f;
    [SerializeField] float maxTornadoCooldown = 8f;
    [SerializeField] float tornadoChargeTime = 3f;
    [SerializeField] GameObject aoeIndicatorPrefab;
    [SerializeField] GameObject abilityPrefab;
    Transform aoeIndicator;
    GameObject fireTornado;
    bool aimingFireTornado;
    bool canUseFireTornado = true;
    Pheonix pheonix;

    [Header("Nav Agent Stuff")]
    [SerializeField] float meleeStopDist = 3f;
    [SerializeField] float rangedStopDist = 10f;
    [SerializeField] float tooCloseDist = 3f;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float rotationSpeed = 1000f;
    float stopSpeed = 0.01f;
    bool isStopped;
    bool aggroChase;

    protected override void Awake()
    {
        base.Awake();
        isInvincible = true;
        player = LevelManager.Instance.PlayerScript as Elana;
        spiritWolfAnimator = GetComponentInChildren<SpiritWolfAnimator>();
        spiritWolf = spiritWolfAnimator.transform;
        pheonix = GetComponentInChildren<Pheonix>();
        updateTargetOnDamaged = false;
    }

    protected override void Update()
    {
        if (!battleStarted) return;
        base.Update();

        if (startAttackingDelay > 0)
        {
            startAttackingDelay -= Time.deltaTime;
            return;
        }

        // Dodge logic - pauses attacks
        if (enableDash && CanDodge() && ShouldDodge())
        {
            StartCoroutine(DodgeRoutine());
        }

        // Ranged logic
        if (enableRange && CanShoot() && ShouldShoot())
        {
            StartCoroutine(RangedRoutine());
        }

        // Tornado logic
        if (enableTornado && CanTornado() && ShouldTornado())
        {
            StartCoroutine(FireTornadoRoutine());
        }

        // Melee logic 
        if (enableMelee && CanMelee() && ShouldMelee())
        {
            StartCoroutine(MeleeRoutine());
        }

        isStopped = agent.velocity.magnitude < stopSpeed;
        if (canSeePlayer) dodgeAwayTimer = dodgeAwayDelay;
        else if (isStopped) dodgeAwayTimer -= Time.deltaTime;

        aggroChase = playerDistance >= aggressiveChaseDist;
        if (aggroChase && !aimingFireTornado && !melee) agent.speed = runSpeed;
        else if (!aggroChase && !aimingFireTornado && !melee) agent.speed = walkSpeed;
    }

    protected override void SlowUpdate()
    {
        base.SlowUpdate();

        playerDistance = Vector3.Distance(player.transform.position, transform.position);
        canSeePlayer = Vector3.Dot(transform.forward, StaticUtilities.FlatDirection(player.transform.position, transform.position)) > maxViewAngle/180;
    }

    protected override void OnHealthZeroed()
    {
        StopAllCoroutines();

        (hp as BossHealthComponent).Hide();

        Destroy(gameObject, 1f);
        LevelManager.Instance.CurrentEncounter.EndEncounter();
    }

    public BossHealthComponent GetHPComponent()
    {
        return hp as BossHealthComponent;
    }

    public void Introduce()
    {
        (hp as BossHealthComponent).Show();

        projectile.CheckPrefab();
        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectile.prefab).GetComponent<MagicBullet>();
            mb.Initialize(projectile, this);
            pooledProjectiles.Add(mb.gameObject);
        }

        battleStarted = true;
        isInvincible = false;
    }

    IEnumerator MeleeRoutine()
    {
        melee = true;
        canUseMelee = false;
        attacksBeingUsed++;
        agent.stoppingDistance = meleeStopDist;

        Vector3 start, end;
        Quaternion startRot, endRot;
        target = player.transform;

        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Wolf Swipe", gameObject);
        

        float attackTime = Random.Range(minMeleeAttackTime, maxMeleeAttackTime);

        bool needsLerp;
        bool playerTooFar;
        bool oldPlayerTooFar = false;

        float lerpTime = 0;
        for (float t = 0; t < attackTime; t += Time.deltaTime)
        {
            while (pauseAttack)
            {
                if (isDodgeing) spiritWolfAnimator.EndAttack();
                yield return new WaitForSeconds(0.2f);
            }
            spiritWolfAnimator.PrimaryAttack();

            playerTooFar = playerDistance > meleeRange;
            if (playerTooFar != oldPlayerTooFar) lerpTime = 0;
            oldPlayerTooFar = playerTooFar;
            needsLerp = lerpTime < 1;
            
            if (!playerTooFar && needsLerp && canSeePlayer)
            {
                start = FindSpiritWolfStartPos();
                end = FindSpiritWolfAttackPos();
                startRot = Quaternion.LookRotation(transform.forward);
                endRot = Quaternion.LookRotation(end - start);
                agent.angularSpeed = 0;
                
                //lerp to
                for (lerpTime = 0; lerpTime < 1 && !playerTooFar; lerpTime += Time.deltaTime * wolfLerpSpeed)
                {
                    spiritWolf.SetPositionAndRotation(Vector3.Lerp(start, end, StaticUtilities.easeCurve01.Evaluate(lerpTime)),
                        Quaternion.Slerp(startRot, endRot, StaticUtilities.easeCurve01.Evaluate(lerpTime * 2f)));
                    yield return null;
                }
            }
            else if (!playerTooFar && !needsLerp && canSeePlayer)
            {
                spiritWolf.position = FindSpiritWolfAttackPos();
                spiritWolf.LookAt(player.transform);
            }
            else if ((playerTooFar && needsLerp) || !canSeePlayer)
            {
                start = FindSpiritWolfStartPos();
                end = GetSpiritWolfPassivePos();
                agent.angularSpeed = rotationSpeed;

                // lerp back
                for (lerpTime = 0; lerpTime < 1 && playerTooFar; lerpTime += Time.deltaTime * wolfLerpSpeed)
                {
                    spiritWolf.position = Vector3.Lerp(start, end, StaticUtilities.easeCurve01.Evaluate(lerpTime));
                    spiritWolf.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
                    yield return null;
                }
            }
            else if (playerTooFar && !needsLerp)
            {
                spiritWolf.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
                if (!isDodgeing) StartCoroutine(DodgeRoutine());
            }

            if (!aimingFireTornado && playerTooFar) agent.speed = runSpeed;
            else if (!aimingFireTornado && !playerTooFar) agent.speed = walkSpeed;

            yield return null;
        }

        if (!aimingFireTornado && isStopped) agent.speed = walkSpeed;
        agent.angularSpeed = rotationSpeed;
        spiritWolfAnimator.EndAttack();
        attacksBeingUsed--;
        melee = false;

        yield return new WaitForSeconds(GetRandomCooldown(minMeleeCooldown, maxMeleeCooldown));
        canUseMelee = true;
        yield break;
    }

    Vector3 FindSpiritWolfAttackPos()
    {
        if (NavMesh.SamplePosition(player.transform.position + 0.5f * transform.right - 0.5f * transform.forward, out NavMeshHit hit2, meleeRange, NavMesh.AllAreas))
        {
            return StaticUtilities.BuildVector(hit2.position.x, player.transform.position.y, hit2.position.z);
        }
        else
        {
            return player.transform.position - transform.forward;
        }
    }

    Vector3 FindSpiritWolfStartPos()
    {
        if (spiritWolfAnimator.IsAttacking)
        {
            return spiritWolf.position;
        }
        else
        {
            return GetSpiritWolfPassivePos();
        }
    }

    Vector3 GetSpiritWolfPassivePos()
    {
        return transform.position + transform.right + transform.up * 0.25f;
    }

    IEnumerator RangedRoutine()
    {
        attacksBeingUsed++;
        canUseRanged = false; 
        if (!melee) agent.stoppingDistance = rangedStopDist;
     
        Vector3 force;
        SetShooting(true);
        // shoot at player in bursts of random amounts
        int shots = Random.Range(minBurst, maxBurst);
        for (int s = 0; s < shots; s++)
        {
            while (pauseAttack || !canSeePlayer)
            {
                if (shooting) SetShooting(false);
                yield return new WaitForSeconds(0.5f);
            }
            if (!shooting) SetShooting(true);

            if (playerDistance <= tooCloseDist && CanDodge()) StartCoroutine(DodgeRoutine());

            force = (player.transform.position - shootOrigin.position).normalized * projectile.speed;
            StaticUtilities.ShootProjectile(pooledProjectiles, shootOrigin.position, force);

            yield return new WaitForSeconds(bulletCooldown);
        }
        SetShooting(false);
        attacksBeingUsed--;

        if (isStopped && !melee && !aimingFireTornado) agent.speed = walkSpeed;
        yield return new WaitForSeconds(GetRandomCooldown(minRangedCooldown, maxRangedCooldown));
        canUseRanged = true; 
    }

    void SetShooting(bool isShooting)
    {
        shooting = isShooting;
        dragonflyAnimator.SetBool("IsShooting", isShooting);
    }

    IEnumerator FireTornadoRoutine()
    {
        // prepare
        attacksBeingUsed++;
        agent.speed = stopSpeed;
        canUseFireTornado = false;

        aoeIndicator = Instantiate(aoeIndicatorPrefab, player.transform.position, aoeIndicatorPrefab.transform.rotation).transform;

        // aim 
        RaycastHit hit;
        float testDist = 10f;
        aimingFireTornado = true;
        pheonix.CastAttack();
        for (float t = 0; t < tornadoChargeTime && canSeePlayer && playerDistance > tooCloseDist; t += Time.deltaTime)
        {
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit, testDist, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
            {
                if (!aoeIndicator.gameObject.activeSelf) aoeIndicator.gameObject.SetActive(true);
                aoeIndicator.position = hit.point;
            }
            else if (aoeIndicator.gameObject.activeSelf) aoeIndicator.gameObject.SetActive(false);
            yield return null;
        }

        // end of cast
        pheonix.EndAttack();
        aimingFireTornado = false;
        attacksBeingUsed--;
        if (isStopped) agent.speed = walkSpeed;

        // spawn on player
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, testDist, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
        {
            fireTornado = Instantiate(abilityPrefab, hit.point, abilityPrefab.transform.rotation);
            fireTornado.GetComponent<FireTornado>().BurnTime = burnTime;
            fireTornado.GetComponent<FireTornado>().Damage = tornadoDamage;
        }
        else
        {
            // cancel
            canUseFireTornado = true;
            Destroy(aoeIndicator.gameObject);
            yield break;
        }

        yield return new WaitForSeconds(tornadoTime);

        fireTornado.GetComponent<VisualEffect>().Stop();
        Destroy(aoeIndicator.gameObject);

        yield return new WaitForSeconds(GetRandomCooldown(minTornadoCooldown, maxTornadoCooldown));

        Destroy(fireTornado);
        canUseFireTornado = true;
    }

    IEnumerator DodgeRoutine()
    {
        // pause current attack
        isDodgeing = isInvincible = pauseAttack = true;
        
        // dodge prepare
        // raycast - make sure there are no obstacles in the way
        float newDist = dodgeDistance;

        Vector3 start = transform.position, end, targetDir, dodgeDir;

        // for if we cant see the player - do a 180
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(-transform.forward);

        // calculate dodge direction
        targetDir = StaticUtilities.FlatDirection(player.transform.position, transform.position);
        if (!canSeePlayer)
        {
            dodgeDir = transform.forward;
        }
        else if (melee || aggroChase)
        {
            dodgeDir = targetDir;
        }
        else
        {
            int side = Random.Range(0, 2) == 0 ? -1 : 1;
            dodgeDir = Vector3.Cross(targetDir * side, Vector3.up);
        }

        if (Physics.Raycast(transform.position, dodgeDir,
            out RaycastHit hit, dodgeDistance, whatIsDodgeObstacle, QueryTriggerInteraction.Ignore))
        {
            newDist = Vector3.Distance(hit.point, transform.position) - 0.5f;
        }

        // get dodge end pos
        end = transform.position + dodgeDir * newDist;

        // lerp
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Wave Dash", gameObject);
        for (float t = 0; t < 1; t += Time.deltaTime * dodgeSpeed)
        {
            if (!canSeePlayer) transform.rotation = Quaternion.Slerp(startRot, endRot, StaticUtilities.easeCurve01.Evaluate(t));
            transform.position = Vector3.Lerp(start, end, StaticUtilities.easeCurve01.Evaluate(t));
            yield return null;
        }

        // dodge end
        pauseAttack = isInvincible = false;
        float cooldown = melee ? minDodgeCooldown : GetRandomCooldown(minDodgeCooldown, maxDodgeCooldown);
        yield return new WaitForSeconds(cooldown);

        isDodgeing = false;
    }

    bool ShouldDodge()
    {
        return (player.IsAttacking() && !aimingFireTornado && !melee) || (isStopped && !canSeePlayer && dodgeAwayTimer <= 0) || aggroChase;
    }

    bool CanDodge()
    {
        return !isDodgeing;
    }

    bool ShouldMelee()
    {
        return !pauseAttack && attacksBeingUsed < maxAttacksAtOnce && !aimingFireTornado;
    }

    bool CanMelee()
    {
        return canUseMelee && !pauseAttack;
    }

    bool ShouldShoot()
    {
        return playerDistance > tooCloseDist && attacksBeingUsed < maxAttacksAtOnce;
    }
    
    bool CanShoot()
    {
        return !pauseAttack && canUseRanged;
    }

    bool ShouldTornado()
    {
        return playerDistance > tooCloseDist && attacksBeingUsed < maxAttacksAtOnce;
    }

    bool CanTornado()
    {
        return canUseFireTornado && !pauseAttack;
    }

    float GetRandomCooldown(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}
