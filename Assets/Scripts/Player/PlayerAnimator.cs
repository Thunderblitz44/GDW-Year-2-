using System;
using UnityEngine;


public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public Rigidbody cc;
    public PlayerMovement playerMovement;
    private bool isTurning = false;
    public Transform spineBone; // Assign the spine bone in the Inspector
       public Transform target;
    private void Awake()
    {
       // cc = GetComponent<Rigidbody>();
      //  playerMovement = GetComponent<PlayerMovement>();

    }

    private void Update()
    {
        if (spineBone != null && target != null && animator != null)
        {
            // Calculate the direction vector from spineBone to target
            Vector3 directionToTarget = target.position - spineBone.position;

            // Create a rotation that aligns the spineBone's forward direction with the direction to the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, spineBone.up);

            // Calculate the signed angle between the forward direction of the spineBone and the target rotation
            float signedAngle = Quaternion.Angle(spineBone.rotation, targetRotation);

            // Ensure the signed angle is in the correct sign
            signedAngle *= Mathf.Sign(Vector3.Cross(spineBone.forward, directionToTarget).y);

            // Convert the signed angle to a range of 0 to 360
            float angle360 = (signedAngle + 360) % 360;

            // Print or use the angle as needed
          //  Debug.Log("Angle: " + angle360);

            // Check if the angle is greater than 180 or less than -180 and trigger the animator accordingly
            if (angle360 > 180 || angle360 < -180)
            {
                if (!isTurning)
                {
                    animator.SetTrigger("TurnTrigger");
                    isTurning = true;
                    Invoke("ResetTurningFlag", 1.0f); // Adjust the delay as needed
                }
            }
        }
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
    void ResetTurningFlag()
    {
        isTurning = false;
    }
    private void FixedUpdate()
    {
      
    }
}
