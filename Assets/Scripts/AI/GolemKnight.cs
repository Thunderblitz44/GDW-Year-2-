using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemKnight : Enemy
{
    // attack
    [SerializeField] float attackDamage = 1f;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float attackDelay = 0.5f;
    float attackTimer;
    float attackCooldownTimer;
    bool attack;

    internal override void Update()
    {
        base.Update();

        // attack cooldown + delay
        attackCooldownTimer += Time.deltaTime;
        if (attackCooldownTimer >= attackCooldown && attack && 
            (attackTimer += Time.deltaTime) >= attackDelay)
        {
            attackCooldownTimer = 0f;
            Attack();
        }
    }

    internal override void OnTriggerEnter(Collider other)
    {
        attack = true;
        animator.SetBool("CanAttack", true);
    }

    internal override void OnTriggerExit(Collider other)
    {
        attack = false;
        attackTimer = 0f;
        animator.SetBool("CanAttack", false);
    }

    public void EnableAI()
    {
        agent.enabled = true;
        target = StaticUtilities.playerTransform;
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
