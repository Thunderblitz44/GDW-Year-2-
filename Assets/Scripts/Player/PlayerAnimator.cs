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

      
        xSpeed = Mathf.Lerp(xSpeed, localVelocity.x, smoothingFactor);
        zSpeed = Mathf.Lerp(zSpeed, localVelocity.z, smoothingFactor);
        ySpeed = Mathf.Lerp(ySpeed, localVelocity.y, smoothingFactor);
       
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
        animator.SetFloat("YSpeed", ySpeed);

    
        
        animator.SetBool("GroundCheck", playerMovement.IsGrounded);
animator.SetBool("IsMoving", playerMovement.IsMoving);
     
        }

    public void IsUsingWolf()
    {
        animator.SetBool("IsUsingWolf", true);
    }
    public void IsNotUsingWolf()
    {
        animator.SetBool("IsUsingWolf", false);
    }
    public void IsUsingDragonFly()
    {
        animator.SetBool("IsUsingDragonFly", true);
    }
    public void IsNotUsingDragonFly()
    {
        animator.SetBool("IsUsingDragonFly", false);
    }
}
