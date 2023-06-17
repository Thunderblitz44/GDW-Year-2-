using System.Collections;
using System.Collections.Generic;
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
        float xSpeed = transform.InverseTransformVector(cc.velocity).x;
        float zSpeed = transform.InverseTransformVector(cc.velocity).z;
        float ySpeed = transform.InverseTransformVector(cc.velocity).y;

        // Set the velocity values in the animator
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
        animator.SetFloat("YSpeed", ySpeed);



        //Jumping/Falling

        // Matches isGrounded with Groundcheck bool in animator
        bool isGrounded = playerMovement.isGrounded;
        animator.SetBool("GroundCheck", isGrounded);
      
    }
}
