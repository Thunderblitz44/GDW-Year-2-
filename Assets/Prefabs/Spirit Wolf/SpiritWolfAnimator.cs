using UnityEngine;

public class SpiritWolfAnimator : MonoBehaviour
{
    private int attackCounter;
    private bool isAttacking;
    public bool IsAttacking { get { return isAttacking; } } 
  
    private Animator animator;
    public int MeleeAttackDamage;
    public Vector2 MeleeKnockback;
    MeleeHitBox[] MeleeAttack;
    // Start is called before the first frame update
    void Start()
    {
        MeleeAttack = GetComponentsInChildren<MeleeHitBox>(true);
        foreach (var trigger in MeleeAttack)
        {
            trigger.damage = MeleeAttackDamage;
            trigger.knockback = MeleeKnockback;
        }

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
      

    }

    public void PrimaryAttack()
    {
        if (isAttacking) return;
        isAttacking = true;
        animator.SetBool("isAttacking", isAttacking);
        animator.SetTrigger("PrimaryAttack");
 
        // Debug.Log(attackCounter);
        //Debug.Log(isAttacking);
    }

    public void AttackCounter()
    {
        attackCounter++;
        if (attackCounter > 3)
        {
            attackCounter = 3;
        }
        animator.SetInteger("attackCounter", attackCounter);
    }

    public void SpecialReset()
    {
        attackCounter = 0;
        animator.SetInteger("attackCounter", attackCounter);
    }
    public void EndAttack()
    {
        if (!isAttacking) return;
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);
        animator.SetInteger("attackCounter", attackCounter);
        //Debug.Log(isAttacking);
    }

 
}
