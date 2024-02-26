using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
   private bool isEnabled = false;
   
    public CapsuleCollider attackTrigger;
    public GameObject HeadTarget;
    MeleeHitBox[] RangedAttack;
    private bool inAttackRange;
    public int RangedAttackDamage;
    public Vector2 RangedKnockback;
    private float xSpeed;
    private float zSpeed;
    // Start is called before the first frame update
    void Start()
    {
        EnableAI();
        RangedAttack = GetComponentsInChildren<MeleeHitBox>(true);
        foreach (var trigger in RangedAttack)
        {
            trigger.damage = RangedAttackDamage;
            trigger.knockback = RangedKnockback;
        }
       
        
    }


    void Update()
    {
        Vector3 headPosition = LevelManager.PlayerTransform.position;
        
        if (isEnabled)
        {
            HeadTarget.transform.position = headPosition;
            agent.SetDestination(headPosition);
            animator.SetBool("IsAttacking", inAttackRange);
        }

        
        float smoothingFactor = 0.1f;

        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity.normalized);

     
        xSpeed = Mathf.Lerp(xSpeed, localVelocity.x, smoothingFactor);
        zSpeed = Mathf.Lerp(zSpeed, localVelocity.z, smoothingFactor);
      
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
    
        
    }
    
    private void EnableAI()
    {
        agent.enabled = true;
        isEnabled = true;
      
        HeadTarget.SetActive(true);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player entered the attack trigger, set inAttackRange to true
            inAttackRange = true;
            // You can also trigger an attack animation here if needed
            // GolemKnightAnimator.SetTrigger("AttackTrigger");
        }
    }

    // Called when another collider exits the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player exited the attack trigger, set inAttackRange to false
            inAttackRange = false;
        }
    }
}
