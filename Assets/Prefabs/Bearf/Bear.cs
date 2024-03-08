using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class Bear : Enemy
{
    [Header("Attack Pattern")]
    [SerializeField] float timeBetweenAttacks = 2;
    [SerializeField] float attackPrepareTime = 2;
    [SerializeField] float maxAttackReps = 1;
    [SerializeField] float startAttackingDelay = 2f;
    readonly List<Func<IEnumerator>> attackFuncs = new();
    int lastAttackIndex;
    float attackReps;
    float atkTimer;
    bool isAttacking;
    bool battleStarted = false;

    [Header("Attacks")]
    [SerializeField] bool enableSlam = true;
    [SerializeField] bool enableOrbs = true;
    [SerializeField] bool enableSpear = true;

    [Header("Slam attack")]
    [SerializeField] float slamCooldown = 1;

    [Header("Orb attack")]
    [SerializeField] GameObject orbPrefab;
    [SerializeField] Transform orbSpawnPoint;
    [SerializeField] float orbCooldown = 1;

    [Header("Spear attack")]
    [SerializeField] GameObject spearPrefab;
    [SerializeField] Transform spearSpawnPoint;
    [SerializeField] GameObject aoeIndicatorPrefab;
    [SerializeField] float spearCooldown = 1;
    Transform aoeIndicator;

    protected override void Update()
    {
        if (!battleStarted) return;
        base.Update();

        if (startAttackingDelay > 0)
        {
            startAttackingDelay -= Time.deltaTime;
            return;
        }

        // timers
        // attack checkers/counters
        // attacks
        if (attackFuncs.Count > 0 && !isAttacking && (atkTimer += Time.deltaTime) > timeBetweenAttacks)
        {
            isAttacking = true;
            atkTimer = 0;
        // pick an attack
        // start coroutine of attack
        newAtkInd:
            int attackIndex = UnityEngine.Random.Range(0, attackFuncs.Count);
            attackReps++;
            if (attackFuncs.Count > 1 && attackIndex == lastAttackIndex && attackReps >= maxAttackReps)
            {
                goto newAtkInd;
            }
            StartCoroutine(attackFuncs[attackIndex]());
            lastAttackIndex = attackIndex;
        }
    }

    public void OrbAttack()
    {
        // Check if orbPrefab and orbSpawnPoint are set
        if (orbPrefab != null && orbSpawnPoint != null)
        {
            // Instantiate the prefab at the specified position and rotation
            Instantiate(orbPrefab, orbSpawnPoint.position, orbSpawnPoint.rotation);
        }
        else
        {
            Debug.LogError("orbPrefab or orbSpawnPoint is not set.");
        }
    }

    public void SpearAttack()
    {
        // Check if orbPrefab and orbSpawnPoint are set
        if (spearPrefab != null && spearSpawnPoint != null)
        {
            // Instantiate the prefab at the specified position and rotation
            Instantiate(spearPrefab, spearSpawnPoint.position, spearSpawnPoint.rotation);
        }
        else
        {
            Debug.LogError("orbPrefab or orbSpawnPoint is not set.");
        }
    }

    public void Introduce()
    {
        hp.SetHealth(hp.MaxHealth);

        attackFuncs.Add(OrbAttackRoutine);
        attackFuncs.Add(SpearAttackRoutine);
        attackFuncs.Add(SlamAttackRoutine);

        battleStarted = true;
        isInvincible = true;
    }

    public BossHealthComponent GetHPComponent()
    {
        return hp as BossHealthComponent;
    }

    protected override void OnHealthZeroed()
    {
        StopAllCoroutines();
        Destroy(gameObject, 1f);
    }

    IEnumerator OrbAttackRoutine()
    {
        if (!enableOrbs)
        {
            isAttacking = false;
            yield break;
        }
        animator.SetTrigger("SummonOrbs");
        yield return new WaitForSeconds(orbCooldown);
        isAttacking = false;
    }

    IEnumerator SpearAttackRoutine()
    {
        if (!enableSpear) 
        {
            isAttacking = false;
            yield break;
        }

        animator.SetTrigger("SummonSpear");
        aoeIndicator = Instantiate(aoeIndicatorPrefab, target.position, aoeIndicatorPrefab.transform.rotation).transform;

        for (float t = 0; t < 4.03f; t += Time.deltaTime)
        {
            aoeIndicator.position = target.position;
            yield return null;
        }

        Destroy(aoeIndicator.gameObject, 3);

        yield return new WaitForSeconds(spearCooldown);
        isAttacking = false;
    }

    IEnumerator SlamAttackRoutine()
    {
        if (!enableSlam)
        {
            isAttacking = false;
            yield break;
        }

        animator.SetTrigger("Slam");
        yield return new WaitForSeconds(slamCooldown);
        isAttacking = false;
    }

}
