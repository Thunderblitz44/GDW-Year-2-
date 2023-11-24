using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // damage applier
    [SerializeField] float damage = 0f;
    [SerializeField] float range = 2f;
    [SerializeField] float angle = 90f;
    [SerializeField] float rate = 1f;
    [SerializeField] float attackDelay = 1;
    float time;

    EnemyAnimator animator;

    const string attackAnimation = StaticUtilities.GOLEM_RANGER_ATTACK;

    public float GetDamage() => damage;
    public float GetRange() => range;
    public float GetAngle() => angle;
    public float GetRate() => rate;

    public bool isAttacking {get; private set;}
    public bool canAttack { get; private set;}

    private void Start()
    {
        animator = gameObject.GetComponent<EnemyAnimator>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time < attackDelay) return;

        if (canAttack && !animator.isAnimationPlaying(1))
        {
            AttackLoop();
        }
        else if (isAttacking)
        {
            time = 0;
            isAttacking = false;
        }
    }


    void AttackLoop()
    {
        isAttacking = true;
        animator.PlayAnimation(attackAnimation);
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
