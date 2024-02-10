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

    [SerializeField] GameObject HeadTarget;


    protected override void Update()
    {
        base.Update();

        HeadTarget.transform.position = LevelManager.PlayerTransform.position;

        // attack cooldown + delay
        attackCooldownTimer += Time.deltaTime;
        if (attackCooldownTimer >= attackCooldown && attack && 
            (attackTimer += Time.deltaTime) >= attackDelay)
        {
            attackCooldownTimer = 0f;
            Attack();
        }
    }

    protected override void OnAttackTriggerEnter(Collider other)
    {
        attack = true;
        animator.SetBool("CanAttack", true);
    }

    protected override void OnAttackTriggerExit(Collider other)
    {
        attack = false;
        attackTimer = 0f;
        animator.SetBool("CanAttack", false);
    }

    public void EnableAI()
    {
        agent.enabled = true;
        target = LevelManager.PlayerTransform;
        HeadTarget.SetActive(true);
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
