using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour, IInputExpander, IPlayerStateListener
{
    // MOVEMENT
    [Header("Movement")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 12f;
    [SerializeField] float groundDrag = 5f;
    float moveSpeed;
    [Space(10f)]

    // 3RD PERSON
    [Header("3rd Person")]
    [SerializeField] float rotationSpeed = 10f;
    [Space(10f)]

    // JUMPING
    [Header("Jumping")]
    [SerializeField] float jumpForce = 12;
    [SerializeField] float airMultiplier = 0.1f;
    [SerializeField] float jumpCooldown = 0.25f;
    bool readyToJump;
    [Space(10f)]


    // CROUCHING
    [Header("Crouching")]
    [SerializeField] float crouchSpeed = 3f;
    [SerializeField] float crouchYScale = .5f;
    float normalYScale;


    // SLOPE
    [Header("Slope Movement")]
    [SerializeField] float maxSlopeAngle = 45f;
    RaycastHit slopeHit;
    bool isExitingSlope;


    // GROUND CHECK
    [Header("Ground Check")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float playerHeight = 2;
    Vector3 velocity;
    float airTime;
    //
    public bool isGrounded;
    //
    bool wasGrounded;
    [Space(10f)]


    // INPUT
    Vector3 inputMoveDirection;
    ActionMap actions;

    // SOME UNORGANIZED DATA
    Rigidbody rb;


    // DELEGATES
    public Action onPlayerLanded;

    // PLAYER
    Player playerScript;

    // PLAYER STATES
    bool isRunning;
    bool isCrouching;
    bool correctBodyRotation;

    [Header("Important")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform combatLookAt;
    [SerializeField] Transform body;

    
    #region Unity Messages

    private void Awake()
    {
        onPlayerLanded += OnLanded;
        rb = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        onPlayerLanded -= OnLanded;
    }

    private void Start()
    {
        rb.freezeRotation = true;
        readyToJump = true;
        normalYScale = transform.localScale.y;
        moveSpeed = walkSpeed;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        SpeedControl();

        if (isGrounded)
        {
            rb.drag = groundDrag;

            // onLanded
            if (!wasGrounded) onPlayerLanded?.Invoke();
        }
        else
        {
            if (wasGrounded) wasGrounded = false;

            rb.drag = 0f;
            airTime += Time.deltaTime;
        }


        // doesnt work
        if (correctBodyRotation)
        {
            body.forward = Vector3.Slerp(body.forward, orientation.forward, Time.deltaTime * rotationSpeed);
            if (body.forward == orientation.forward)
            {
                //Debug.Log("dnoafg");
                correctBodyRotation = false;
            }
        }
    }

    private void LateUpdate()
    {
        velocity = rb.velocity;
    }

    private void FixedUpdate()
    {
        Move();
    }


    #endregion

    #region Inputs

    // call in awake
    public void SetupInputEvents(object sender, ActionMap actions)
    {
        this.actions = actions;
        playerScript = (Player)sender;

        // Movement
        actions.Locomotion.Move.performed += ctx =>
        {
            if (playerScript.isInCombat == false) orientation.forward = CalculateBodyRotation();

            Vector2 input = ctx.ReadValue<Vector2>();

            inputMoveDirection = new Vector3(input.x, 0, input.y);

            UpdateMoveSpeed();
        };

        actions.Locomotion.Move.canceled += ctx => 
        {
            inputMoveDirection = Vector3.zero;
        };


        // Run
        actions.Locomotion.Run.started += ctx =>
        {
            Run();

            UpdateMoveSpeed();
        };

        actions.Locomotion.Run.canceled += ctx =>
        {
            StopRun();

            UpdateMoveSpeed();
        };


        // Crouch
        actions.Locomotion.Crouch.started += ctx =>
        {
            Crouch();
          
            UpdateMoveSpeed();
        };

        actions.Locomotion.Crouch.canceled += ctx =>
        {
            UnCrouch();
          
            UpdateMoveSpeed();
        };


        // Jump
        actions.Locomotion.Jump.started += ctx =>
        {
            if (CanJump()) Jump();
        };


        

        EnableLocomotion();
    }


    #endregion

    #region Locomotion

    void Move()
    {
        Vector3 moveDirection = orientation.forward * inputMoveDirection.z + orientation.right * inputMoveDirection.x;

        if (!playerScript.isInCombat) body.forward = Vector3.Slerp(body.forward, moveDirection.normalized, Time.deltaTime * rotationSpeed);

        if (OnSlope() && !isExitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0f) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        if (isGrounded) rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        else rb.AddForce(moveDirection * moveSpeed * airMultiplier * 10f, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    void Run()
    {
        // override crouching
        if (isCrouching) UnCrouch();

        isRunning = true;
    }

    void StopRun()
    {
        isRunning = false;
    }

    void Crouch()
    {
        // override run
        if (isRunning) StopRun();

        isCrouching = true;
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        if (isGrounded) rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    void UnCrouch()
    {
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, normalYScale, transform.localScale.z);
    }

    public bool CanJump()
    {
        return isGrounded && readyToJump;
    }

    void Jump()
    {
        // prevent spamming
        readyToJump = false;
        isExitingSlope = true;

        // stop any up/down movement
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // jump
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    #endregion

    #region Locomotion Management

    private void SpeedControl()
    {
        // if on slope
        if (OnSlope() && !isExitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(inputMoveDirection, slopeHit.normal).normalized;
    }


    void UpdateMoveSpeed()
    {
        if (isGrounded)
        {
            if (!isRunning)
            {
                if (isCrouching)
                {
                    SetToCrouch();
                }
                else
                {
                    SetToWalk();
                }
            }
            else
            {
                SetToRun();
            }
        }
    }

    Vector3 CalculateBodyRotation()
    {
        Vector3 viewDir;
        Vector3 cameraPosition = playerScript.GetCameraControllerScript().GetCameraTransform().position;

        if (playerScript.isInCombat)
        {
            viewDir = combatLookAt.position - new Vector3(cameraPosition.x, combatLookAt.position.y, cameraPosition.z);
            transform.forward = viewDir.normalized;
            return viewDir.normalized;
        }
        
        viewDir = transform.position - new Vector3(cameraPosition.x, transform.position.y, cameraPosition.z);
        return viewDir.normalized;
    }

    public void RecalculateBodyRotation()
    {
        orientation.forward = CalculateBodyRotation();
    }

    #endregion

    #region Events

    void OnLanded()
    {
        wasGrounded = true;

        if (jumpCooldown > 0) Invoke(nameof(ResetJump), jumpCooldown);
        else ResetJump();

        Debug.Log("air time = " + airTime);

        airTime = 0f;
        velocity = Vector3.zero;
    }

    #endregion

    #region Player Locomotion States

    void SetToWalk()
    {
        moveSpeed = walkSpeed;
    }

    void SetToRun()
    {
        moveSpeed = runSpeed;
    }

    void SetToCrouch()
    {
        moveSpeed = crouchSpeed;
    }

    #endregion

    #region Getters/Setters

    public bool IsMoving() => inputMoveDirection != Vector3.zero;
    public bool IsRunning() => isRunning;
    public bool IsGrounded() => isGrounded;
    public bool IsCrouching() => isCrouching;
    public bool IsJumping() => readyToJump == false;
    public float GetMoveSpeed() => moveSpeed;
    public Vector3 GetVelocity() => velocity;
    public float GetAirTime() => airTime;
    public Transform GetOrientation() => orientation;

    // Controls Toggles
    public void EnableLocomotion() => actions.Locomotion.Enable();
    public void DisableLocomotion() => actions.Locomotion.Disable();
    

    #endregion


    #region IPlayerStateListener

    public void SetCombatState()
    {
        Debug.Log("Combat");
        correctBodyRotation = true;
    }

    public void SetFreeLookState()
    {
        Debug.Log("freelok");
    }

    #endregion
}
