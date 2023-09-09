using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour, IInputExpander
{
    // MOVEMENT
    [Header("Movement")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 12f;
    [SerializeField] float groundDrag = 5f;
    [SerializeField] AnimationCurve runSpeedCurve;
    [SerializeField] float runAccelMultiplier = 1f;
    [SerializeField] float runDecelMultiplier = 1f;
    float moveSpeed;
    float runAccelTime;
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


    // GROUND CHECK
    [Header("Ground Check")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float playerHeight = 2;
    float airTime;
    public bool isGrounded;
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
    bool isAiming;
    bool ignoreStates;

    [Header("Important")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform body;

    
    #region Unity Messages

    private void Awake()
    {
        onPlayerLanded += OnLanded;
        rb = GetComponent<Rigidbody>();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
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
        if (ignoreStates || !IsOwner) return;

        GroundCheck();

        SpeedControl();

        if (isGrounded)
        {
            // onLanded
            if (!wasGrounded) onPlayerLanded?.Invoke();
        }
        else
        {
            if (wasGrounded) wasGrounded = false;

            airTime += Time.deltaTime;
            DebugHUD.instance.SetAirTime(airTime);
        }

        if (isAiming)
        {
            Vector3 forward = new Vector3(transform.position.x + Camera.main.transform.forward.x, 
                transform.position.y, transform.position.z + Camera.main.transform.forward.z);
            transform.LookAt(forward, Vector3.up);
            body.rotation = transform.rotation;
        }

        // double check in case we are still grounded after jumping (it can happen)
        if (isGrounded && !CanJump()) onPlayerLanded();

        DebugHUD.instance.SetSpeed(rb.velocity.magnitude);
    }

    void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position,0.3f,Vector3.down, out hit, playerHeight * 0.5f + 0.05f, whatIsGround, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true; 
            return;
        }
        isGrounded = false;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

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
            if (!isAiming) orientation.forward = CalculateBodyRotation();
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

        actions.CameraControl.Aim.started += ctx => isAiming = true;
        actions.CameraControl.Aim.canceled += ctx => isAiming = false;

        EnableLocomotion();
    }


    #endregion

    #region Locomotion

    void Move()
    {
        Vector3 moveDirection;

        // set move direction - normal operation
        if (!playerScript.GetCameraControllerScript().IsLockedOnToATarget())
        {
            Vector3 cameraFwd = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraFwd.y = 0f;
            cameraRight.y = 0f;
            moveDirection = cameraFwd * inputMoveDirection.z + cameraRight * inputMoveDirection.x;
        }
        else
        {
            Transform camera = Camera.main.transform;
            moveDirection = new Vector3(camera.forward.x, 0, camera.forward.z) * inputMoveDirection.z +
                new Vector3(camera.right.x, 0, camera.right.z) * inputMoveDirection.x;
        }

        // ignore the rest of this function if we are trying to move into an obstacle while jumping
        if (Physics.Raycast(transform.position, moveDirection, 0.6f, LayerMask.NameToLayer("Everything"), QueryTriggerInteraction.Ignore) && 
            !IsJumping() && !isGrounded) return;

        // adjust move direction for slopes
        if (OnSlope()) moveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        // rotate the body
        if (!isAiming) body.forward = Vector3.Slerp(body.forward, moveDirection.normalized, Time.deltaTime * rotationSpeed);

        // move
        if (isGrounded) rb.AddForce(moveDirection * moveSpeed * rb.mass * 10f, ForceMode.Force);
        else rb.AddForce(moveDirection * moveSpeed * airMultiplier * rb.mass * 10f, ForceMode.Force);
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
        if (isGrounded) rb.AddForce(Vector3.down * 5f * rb.mass, ForceMode.Impulse);
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

        // stop any up/down movement
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // jump
        rb.AddForce(Vector3.up * jumpForce * rb.mass, ForceMode.Impulse);
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
        if (OnSlope())
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

        if (isRunning)
        {
            runAccelTime += Time.deltaTime * runAccelMultiplier;
            moveSpeed = Mathf.Lerp(walkSpeed, runSpeed, runSpeedCurve.Evaluate(runAccelTime));
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight, whatIsGround, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle <= maxSlopeAngle && angle > 0;
        }
        return false;
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
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 viewDir = transform.position - new Vector3(cameraPosition.x, transform.position.y, cameraPosition.z);
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
        airTime = 0f;
    }

    #endregion

    #region Player Locomotion States

    void SetToWalk()
    {
        moveSpeed = walkSpeed;

    }

    void SetToRun()
    {
        //moveSpeed = runSpeedCurve.Evaluate(0);
        runAccelTime = 0f;
    }

    void SetToCrouch()
    {
        moveSpeed = crouchSpeed;
    }

    public void Disable()
    {
        ignoreStates = true;
        DisableLocomotion();
    }

    public void Enable()
    {
        ignoreStates = false;
        EnableLocomotion();
    }

    #endregion

    #region Getters/Setters

    public bool IsMoving() => inputMoveDirection != Vector3.zero;
    public bool IsRunning() => isRunning;
    public bool IsGrounded() => isGrounded;
    public bool IsCrouching() => isCrouching;
    public bool IsJumping() => readyToJump == false;
    public float GetMoveSpeed() => moveSpeed;
    public float GetAirTime() => airTime;
    public Transform GetOrientation() => orientation;
    public Rigidbody GetRigidbody() => rb;
    public Transform GetBody() => body;
    public Vector3 GetInputMoveDirection() => inputMoveDirection;

    // Controls Toggles
    public void EnableLocomotion() => actions.Locomotion.Enable();
    public void DisableLocomotion() => actions.Locomotion.Disable();
    

    #endregion
}
