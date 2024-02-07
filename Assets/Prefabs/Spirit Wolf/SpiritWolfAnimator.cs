using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritWolfAnimator : MonoBehaviour
{
    private int attackCounter;
    private bool isAttacking;
    
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
       attackCounter++;
       animator.SetBool("isAttacking", isAttacking);
       animator.SetTrigger("PrimaryAttack");
       animator.SetInteger("attackCounter", attackCounter);
      // Debug.Log(attackCounter);
      //Debug.Log(isAttacking);
    }


   public void EndAttack()
   {
       isAttacking = false;
       animator.SetBool("isAttacking", isAttacking);
       //Debug.Log(isAttacking);
   }
}
