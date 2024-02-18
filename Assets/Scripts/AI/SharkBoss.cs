using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkBoss : Enemy, IBossCommands
{
    [Header("Attack Pattern")]
    [SerializeField] float timeBetweenAttacks = 2;
    [SerializeField] float attackPrepareTime = 2;
    [SerializeField] float maxAttackReps = 2;
    float attackReps;
    int lastAttackIndex;
    int phase = 0;
    float atkTimer;
    bool isAttacking;
    readonly List<Func<IEnumerator>> attackFuncs = new();

    bool battleStarted = false;

    public void Introduce()
    {
        (hp as BossHealthComponent).Show();
        Invoke(nameof(StartBattle), 1f);
        NextPhase();
    }

    protected override void Awake()
    {
        base.Awake();
        //EnableAI();
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

    void StartBattle()
    {
        battleStarted = true;
        target = LevelManager.PlayerTransform;
    }

    void NextPhase()
    {
        switch (++phase)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }
}
