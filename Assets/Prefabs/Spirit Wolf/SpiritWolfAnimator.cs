using UnityEngine;

public class SpiritWolfAnimator : MonoBehaviour
{
    private int attackCounter;
    private bool isAttacking;
    [SerializeField] MeleeHitBox meleeHitBoxL;
    [SerializeField] MeleeHitBox meleeHitBoxR;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
      

    }

    public void PrimaryAttack()
    {
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
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);
        animator.SetInteger("attackCounter", attackCounter);
        //Debug.Log(isAttacking);
    }

    public void AttackEventLeft()
    {
        meleeHitBoxL.ReadyAttack();
        
    }
    public void AttackEventLeftHide()
    {
        meleeHitBoxL.Hide();
        
    }
    public void AttackEventRight()
    {
        meleeHitBoxR.ReadyAttack();
    }
    
    public void AttackEventRightHide()
    {
        meleeHitBoxR.Hide();
        
    }
}
