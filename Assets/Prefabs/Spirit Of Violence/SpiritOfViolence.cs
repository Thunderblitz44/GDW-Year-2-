using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiritOfViolence : Enemy, IBossCommands
{
    //* PART 2 - ELANA DOPPLEGANGER - *//
    
    [Header("Attack Pattern")]
    [SerializeField] float timeBetweenAttacks = 2;
    [SerializeField] float attackPrepareTime = 2;
    [SerializeField] float maxAttackReps = 2;
    float attackReps;
    int lastAttackIndex;
    float atkTimer;
    bool isAttacking;
    bool canPauseAttack;
    bool pauseAttack;
    bool interruptAttack;
    readonly List<Func<IEnumerator>> attackFuncs = new();
    bool battleStarted = false;
    Elana player;

    [Header("Melee Attack")]
    [SerializeField] float meleeRange = 6f;
    [SerializeField] float wolfLerpSpeed = 1;
    [SerializeField] float minMeleeAttackTime = 1;
    [SerializeField] float maxMeleeAttackTime = 5;
    [SerializeField] SpiritWolfAnimator spiritWolfAnimator;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] bool enableMelee;
    Transform spiritWolf;
    float playerDistance = 1000;
    float wolfLerpTime;
    bool lerpWolf;
    bool melee;

    [Header("Ranged Attack")]
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] float shootStartDelay = 0.5f;
    [SerializeField] float bulletCooldown = 0.25f;
    [SerializeField] Transform shootOrigin;
    [SerializeField] int minBurst = 10;
    [SerializeField] int maxBurst = 30;
    [SerializeField] public Animator dragonflyAnimator;
    [SerializeField] bool enableRange;
    readonly List<GameObject> pooledProjectiles = new(20);
    bool shooting;
    float shootingCooldownTimer;
    float shootStartTimer;

    [Header("Dodge")]
    [SerializeField] float dodgeDistance = 6f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] float minDodgeCooldown = 1f;
    [SerializeField] float maxDodgeCooldown = 5f;
    [SerializeField] LayerMask whatIsDodgeObstacle;
    [SerializeField] bool enableDash;
    bool isDodgeing = false;

    [Header("Fire Tornado")]
    [SerializeField] float maxRange = 15f;
    [SerializeField] int tornadoDamage = 1;
    [SerializeField] float burnTime = 3f;
    [SerializeField] float tornadoTime = 5f;
    [SerializeField] float tornadoCooldown = 2f;
    [SerializeField] GameObject aoeIndicatorPrefab;
    [SerializeField] GameObject abilityPrefab;
    [SerializeField] bool enableTornado;
    Transform aoeIndicator;
    GameObject fireTornado;
    bool aimingFireTornado = false;
    bool canUseFireTornado = true;
    [SerializeField] private Pheonix pheonix;

    protected override void Awake()
    {
        base.Awake();
        player = LevelManager.Instance.PlayerScript as Elana;
        spiritWolf = spiritWolfAnimator.transform;
    }

    protected override void Update()
    {
        if (!battleStarted) return;
        base.Update();

        // timers
        // attack checkers/counters
        // attacks
        if (attackFuncs.Count > 0 && !isAttacking && (atkTimer += Time.deltaTime) > timeBetweenAttacks)
        {
            isAttacking = true;
            atkTimer = 0;
        // pick an attack
        // start coroutine of that attack
        newAtkInd:
            int attackIndex = UnityEngine.Random.Range(0, attackFuncs.Count);
            if (attackFuncs.Count > 1 && attackIndex == lastAttackIndex && ++attackReps >= maxAttackReps)
            {
                goto newAtkInd;
            }
            StartCoroutine(attackFuncs[attackIndex]());
            lastAttackIndex = attackIndex;
        }


        // Dodge logic
        if (enableDash && CanDodge() && ShouldDodge())
        {
            isDodgeing = isInvincible = true;
            StartCoroutine(DodgeRoutine());
        }

        // Melee logic - interrupts the other attacks
        if (enableMelee && !pauseAttack && playerDistance < meleeRange && (!isAttacking || IsAttackPauseable()))
        {
            isAttacking = interruptAttack = true;
            StartCoroutine(MeleeRoutine());
        }
    }

    protected override void SlowUpdate()
    {
        base.SlowUpdate();

        playerDistance = Vector3.Distance(player.transform.position, transform.position);
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
        attackFuncs.Add(RangedRoutine);
        //attackFuncs.Add(FireTornadoRoutine);

        projectile.CheckPrefab();
        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectile.prefab).GetComponent<MagicBullet>();
            mb.Initialize(projectile, this);
            pooledProjectiles.Add(mb.gameObject);
        }

        battleStarted = true;
    }

    IEnumerator MeleeRoutine()
    {
        canPauseAttack = true;
        Vector3 start, end;
        Quaternion startRot, endRot;
        yield return null;
        melee = true;

        //Animate player/wolf attacking in sync
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Wolf Swipe", gameObject);
        spiritWolfAnimator.PrimaryAttack();
        start = transform.position + transform.right + transform.up * 0.25f;
        end = FindSpiritWolfAttackPos();

        startRot = Quaternion.LookRotation(transform.forward);
        endRot = Quaternion.LookRotation(end - start);

        // lerp
        for (float t = 0; t < 1; t += Time.deltaTime * wolfLerpSpeed)
        {
            spiritWolf.SetPositionAndRotation(Vector3.Lerp(start, end, StaticUtilities.easeCurve01.Evaluate(t)),
                Quaternion.Slerp(startRot, endRot, StaticUtilities.easeCurve01.Evaluate(t * 2f)));

            if (playerDistance > meleeRange) goto end;
            yield return null;
        }

        float attackTime = UnityEngine.Random.Range(minMeleeAttackTime, maxMeleeAttackTime);
        for (float t = 0; t < attackTime && playerDistance < meleeRange; t += Time.deltaTime)
        {
            spiritWolf.position = FindSpiritWolfAttackPos();
            spiritWolf.LookAt(player.transform);
            yield return null;
        }

    end:
        spiritWolfAnimator.EndAttack();
        yield return new WaitForSeconds(2f);

        melee = false;
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

    IEnumerator RangedRoutine()
    {
        if (!enableRange) yield break;
        isAttacking = canPauseAttack = true;
        Vector3 force;

        SetShooting(true);
        // shoot at player in bursts of random amounts
        int shots = UnityEngine.Random.Range(minBurst, maxBurst);
        for (int s = 0; s < shots; s++)
        {
            while (pauseAttack)
            {
                if (shooting) SetShooting(false);
                yield return new WaitForSeconds(0.5f);
            }
            if (interruptAttack) 
            {
                SetShooting(false);
                yield break;
            }
            if (!shooting) SetShooting(true);

            force = (player.transform.position - shootOrigin.position).normalized * projectile.speed;
            StaticUtilities.ShootProjectile(pooledProjectiles, shootOrigin.position, force);

            yield return new WaitForSeconds(bulletCooldown);
        }

        SetShooting(false);
        isAttacking = false;
        canPauseAttack = false;
        yield break;
    }

    void SetShooting(bool isShooting)
    {
        shooting = isShooting;
        dragonflyAnimator.SetBool("IsShooting", isShooting);
    }

    IEnumerator FireTornadoRoutine()
    {
        // stop moving
        // aim fire tornado
        // spawn fire tornado on player

        yield break;
    }

    IEnumerator DodgeRoutine()
    {
        // pause current attack
        pauseAttack = true;
        
        // dodge prepare
        // raycast - make sure there are no obstacles in the way
        float newDist = dodgeDistance;

        Vector3 start = transform.position, end, targetDir, dodgeDir;

        // calculate dodge direction
        int side = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        targetDir = StaticUtilities.FlatDirection(player.transform.position, transform.position);
        dodgeDir = Vector3.Cross(targetDir * side, Vector3.up);

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
            transform.position = Vector3.Lerp(start, end, StaticUtilities.easeCurve01.Evaluate(t));
            yield return null;
        }

        // dodge end
        pauseAttack = isInvincible = false;

        yield return new WaitForSeconds(UnityEngine.Random.Range(minDodgeCooldown, maxDodgeCooldown));
        isDodgeing = false;
    }

    bool ShouldDodge()
    {
        return player.IsAttacking();
    }

    bool CanDodge()
    {
        return !isDodgeing && (!isAttacking || IsAttackPauseable());
    }

    bool IsAttackPauseable()
    {
        return isAttacking && canPauseAttack;
    }
}
