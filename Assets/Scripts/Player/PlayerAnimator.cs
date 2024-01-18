using System;
using UnityEngine;


public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
   public PlayerMovement playerMovement;
 
    private void Start()
    {
       
animator = GetComponent<Animator>();

playerMovement = playerMovement.GetComponent<PlayerMovement>();


    }

  
    private void FixedUpdate()
    { 
      
        // Get the current velocity components
        float xSpeed = transform.InverseTransformDirection(playerMovement.rb.velocity).x;
        float zSpeed = transform.InverseTransformDirection(playerMovement.rb.velocity).z;
        float ySpeed = transform.InverseTransformDirection(playerMovement.rb.velocity).y;

        // Set the velocity values in the animator
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
        animator.SetFloat("YSpeed", ySpeed);

        animator.SetBool("PrimaryAttackBool", Elana.isPrimaryAttacking);
        
        // Matches isGrounded with Groundcheck bool in animator
        
        animator.SetBool("GroundCheck", playerMovement.isGrounded);

     
        }


 
}
