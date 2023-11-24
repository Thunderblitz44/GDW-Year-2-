using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // damage applier
    [SerializeField] float damage = 0f;
    [SerializeField] float range = 2f;
    [SerializeField] float angle = 90f;
    [SerializeField] float rate = 1f;
    [SerializeField] float attackDelay = 1;

    float attackTime = 0;

    public float GetDamage() => damage;
    public float GetRange() => range;
    public float GetAngle() => angle;
    public float GetRate() => rate;

    public bool isAttacking {get; private set;}
    public bool canAttack { get; private set;}

    private void Update()
    {
        attackTime += Time.deltaTime;
        if (attackTime < attackDelay || !canAttack) return;
        attackTime = 0;

        AttackLoop();
    }


    void AttackLoop()
    {
        Debug.Log("attack");
    }


    public void Attack()
    {
        canAttack = true;
    }


    public void StopAttack()
    {
        canAttack = false;
    }

}
