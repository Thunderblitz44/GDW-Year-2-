using System;
using UnityEngine;


public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public Rigidbody cc;
    public PlayerMovement playerMovement;
  
   
    private void Awake()
    {
       // cc = GetComponent<Rigidbody>();
      //  playerMovement = GetComponent<PlayerMovement>();

    }

    private void Update()
    {
        //Moving Around

        // Get the current velocity components
        float xSpeed = transform.InverseTransformDirection(cc.velocity).x;
        float zSpeed = transform.InverseTransformDirection(cc.velocity).z;
        float ySpeed = transform.InverseTransformDirection(cc.velocity).y;

        // Set the velocity values in the animator
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
        animator.SetFloat("YSpeed", ySpeed);



        //Jumping/Falling

        // Matches isGrounded with Groundcheck bool in animator
        bool isGrounded = playerMovement.isGrounded;
        animator.SetBool("GroundCheck", isGrounded);

     
        }
    

    private void FixedUpdate()
    {
      
    }
}
