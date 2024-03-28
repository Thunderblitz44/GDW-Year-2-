using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
   public PlayerMovement playerMovement;
   float xSpeed = 0f;
   float zSpeed = 0f;
   float ySpeed = 0f;

    private void Start()
    {
       
        // Subscribe to the OnPlayerDeath event
        playerMovement.OnPlayerDeath += Death;
animator = GetComponent<Animator>();

playerMovement = playerMovement.GetComponent<PlayerMovement>();


    }

    private void Death()
    {
        animator.SetTrigger("Die");
        
 
    }


    private void FixedUpdate()
    { 
        float smoothingFactor = 0.1f;

        Vector3 localVelocity = transform.InverseTransformDirection(playerMovement.Rb.velocity);

        // Smooth the velocity components (remove the float keyword)
        xSpeed = Mathf.Lerp(xSpeed, localVelocity.x, smoothingFactor);
        zSpeed = Mathf.Lerp(zSpeed, localVelocity.z, smoothingFactor);
        ySpeed = Mathf.Lerp(ySpeed, localVelocity.y, smoothingFactor);
        // Set the velocity values in the animator
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
        animator.SetFloat("YSpeed", ySpeed);

    
        
        // Matches isGrounded with Groundcheck bool in animator
        
        animator.SetBool("GroundCheck", playerMovement.IsGrounded);
animator.SetBool("IsMoving", playerMovement.IsMoving);
     
        }


 
}
