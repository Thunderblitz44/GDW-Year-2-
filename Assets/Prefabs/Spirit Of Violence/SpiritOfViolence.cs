using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    readonly List<Func<IEnumerator>> attackFuncs = new();
    bool battleStarted = false;

    [Header("Melee Attack")]
    [SerializeField] float range = 6f;
    [SerializeField] private SpiritWolfAnimator spiritWolfAnimator;
    [SerializeField] float wolfLerpSpeed = 1;
    Transform spiritWolf;
    bool melee;
    bool lerpWolf;
    [SerializeField] LayerMask enemyLayer;
    Vector3 wolfLerpStart;
    Vector3 wolfLerpEnd;
    Quaternion wolfRotStart;
    Quaternion wolfRotEnd;
    float wolfLerpTime;

    [Header("Ranged Attack")]
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] float shootStartDelay = 0.5f;
    [SerializeField] float bulletCooldown = 0.25f;
    [SerializeField] Transform shootOrigin;
    readonly List<GameObject> pooledProjectiles = new(20);
    bool shooting;
    float shootingCooldownTimer;
    float shootStartTimer;

    [Header("Dash")]
    [SerializeField] float dodgeDistance = 5f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] float dodgeCooldown = 1f;
    [SerializeField] AnimationCurve dodgeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] LayerMask whatIsDodgeObstacle;
    bool isDodgeing = false;
    bool canDodge = true;
    [SerializeField] TrailScript TrailScript;

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
    [SerializeField] private Pheonix pheonix;

    protected override void Awake()
    {
        base.Awake();
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
    }

    IEnumerator MeleeRoutine()
    {
        // if we are close enough to player
        // lerp spirit wolf to player
        // attack player

        yield break;
    }

    IEnumerator RangedRoutine()
    {
        // shoot at player in bursts of random amounts

        yield break;
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
        // detect if the player is attacking
        // if we can dodge, then dodge
        // random cooldown time
        
        yield break;
    }
}
