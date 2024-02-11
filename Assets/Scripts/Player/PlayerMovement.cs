using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IInputExpander
{
    // MOVEMENT
    [Header("Movement")]
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float walkAcceleration = 1f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float runAcceleration = 4f;
    //[SerializeField] float runSpeedLR = 5f;
    [SerializeField] float airSpeed = 3f;
    [SerializeField] float airAcceleration = 3f;
    bool isRunning;
    bool stepClimbing;
    public bool IsMoving { get { return input != Vector3.zero; } }
    public Vector3 MoveDirection { get { return moveDirection; } }
    float MoveSpeed { get { return IsGrounded ? isRunning ? runSpeed : walkSpeed : airSpeed; } }
    float MoveAcceleration { get { return IsGrounded ? isRunning ? runAcceleration : walkAcceleration : airAcceleration; } }
    [Space(10f)]

    // JUMPING
    [Header("Jumping")]
    [SerializeField] float jumpForce = 12;
    [SerializeField] float jumpCooldown = 0.25f;
    bool canJump = true;
    bool jumped;
    float jumpCooldownTimer;
    [Space(10f)]

    // GROUND CHECK
    [Header("Ground Check")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float maxSlopeAngle = 45f;
    [SerializeField] float groundRaycastDist = 0.2f;
    Transform groundCheck;
    Transform kneeClearanceCheck;
    RaycastHit ground;
    float groundAngle;
    public bool IsGrounded { get; private set; }
    int collisions;
    [Space(10f)]

    // INPUT
    Vector3 input;
    Vector3 moveDirection;
    ActionMap actions;

    // PLAYER STUFF
    PlayerCollisions playerCollisions;
    public Transform Body { get; private set; }
    public Rigidbody Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Body = transform.GetChild(0);

        for (int c = 0; c < Body.childCount; c++)
        {
            string name = Body.GetChild(c).name;
            if (name == "KneeClearance") kneeClearanceCheck = Body.GetChild(c);
            else if (name == "GroundChecker") groundCheck = Body.GetChild(c);
        }

        playerCollisions = Body.GetComponent<PlayerCollisions>();
        playerCollisions.onCollisionStay += OnCollisionStay;
        playerCollisions.onCollisionExit += OnCollisionExit;
        playerCollisions.onCollisionEnter += OnCollisionEnter;
    }

    private void Update()
    {
        if (collisions == 0 && IsGrounded && !stepClimbing && (jumped || Mathf.Abs(Rb.velocity.y) > 2f)) IsGrounded = false;
        // can jump test
        if (IsGrounded && (jumpCooldownTimer += Time.deltaTime) >= jumpCooldown) canJump = true;
        if (jumped && IsGrounded && canJump) jumped = false;

        // rotate body
        Body.rotation = Quaternion.LookRotation(StaticUtilities.GetCameraDir());

        DebugHUD.instance.SetSpeed(Rb.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        // do move
        if (IsGrounded && Rb.velocity.magnitude < MoveSpeed)
        {
            Vector3 rotatedInput = StaticUtilities.GetCameraDir() * input.z + StaticUtilities.HorizontalizeVector(Camera.main.transform.right) * input.x;
            moveDirection = rotatedInput;
            if (IsGrounded && groundAngle < maxSlopeAngle)
            {
                moveDirection = Vector3.ProjectOnPlane(rotatedInput, ground.normal);
            }
            Rb.velocity += moveDirection * MoveAcceleration;
        }
        else if (!IsGrounded && StaticUtilities.HorizontalizeVector(Rb.velocity).magnitude < MoveSpeed)
        {
            moveDirection = StaticUtilities.GetCameraDir() * input.z + StaticUtilities.HorizontalizeVector(Camera.main.transform.right) * input.x;
            Rb.velocity += moveDirection * MoveAcceleration;
        }

        // Steps
        RaycastHit hitLower;
        if (Physics.Raycast(groundCheck.position, moveDirection, out hitLower, 0.25f))
        {
            if (Vector3.Dot(hitLower.normal, moveDirection) < -0.7f && !Physics.Raycast(kneeClearanceCheck.position, moveDirection, 0.4f))
            {
                stepClimbing = true;
                Rb.position += Vector3.up * 0.05f;
            }
        }
        else if (stepClimbing) stepClimbing = false;
    }

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        this.actions = actions;

        // Movement
        actions.Locomotion.Move.performed += ctx =>
        {
            // convert wasd to 3D direction
            Vector2 input = ctx.ReadValue<Vector2>();
            this.input = Vector3.right * input.x + Vector3.forward * input.y;

            if (input.y < 0.5f && isRunning) isRunning = false;
        };

        actions.Locomotion.Move.canceled += ctx =>
        {
            input = Vector3.zero;
            isRunning = false;
        };

        // Run
        actions.Locomotion.Run.performed += ctx =>
        {
            if (Mathf.Abs(input.x) == 1 || input.z < 0) return;
            isRunning = true;
        };


        // Jump
        actions.Locomotion.Jump.started += ctx =>
        {
            if (jumped || !IsGrounded) return;
            jumpCooldownTimer = 0;
            jumped = true;
            canJump = false;
            Rb.velocity = StaticUtilities.HorizontalizeVector(Rb.velocity);
            Rb.velocity += Vector3.up * jumpForce;
        };
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;
    }

    private void OnCollisionStay(Collision collision)
    {
        // is grounded test
        // are we colliding with an object lower than groundcheck
        if (collision.collider.gameObject.layer == 6 &&
            collision.contacts[0].point.y < groundCheck.position.y + 1.5f)
        {
            if (Physics.Raycast(groundCheck.position, Vector3.down, out ground, groundRaycastDist, groundLayer, QueryTriggerInteraction.Ignore))
            {
                groundAngle = Vector3.Angle(Vector3.up, ground.normal);
                DebugHUD.instance.SetDebugText("ground angle : " + groundAngle.ToString("0.0"));

                IsGrounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        collisions--;
    }

    public void EnableLocomotion() => actions.Locomotion.Enable();
    public void DisableLocomotion() => actions.Locomotion.Disable();
}

    /*// MOVEMENT
    [Header("Movement")]
    [SerializeField] float walkForce = 3f;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runForce = 3f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float runSpeedLR = 5f;
    [SerializeField] float airborneForce = 0.5f;
    [SerializeField] float airborneXYSpeed = 1f;
    float moveForce;
    float runAccelTime;
    bool isRunning;
    bool wasRunning;
    bool ignoreMovement;
    float oldDrag;
    float temp;
    [Space(10f)]

    // JUMPING
    [Header("Jumping")]
    [SerializeField] float jumpForce = 12;
    [SerializeField] float jumpCooldown = 0.25f;
    bool readyToJump = true;
    bool isJumping = false;
    GameObject wallobj;
    [Space(10f)]

    // GROUND CHECK
    [Header("Ground Check")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float playerHeight = 2;
    [SerializeField] float maxSlopeAngle = 45f;
    RaycastHit ground;
    float airTime;
    public bool isGrounded = true;
    bool wasGrounded = true;
    float groundAngle;
    [Space(10f)]

    // INPUT
    Vector3 input;
    Vector3 oldInput;
    Vector3 moveDirection;
    ActionMap actions;

    // SOME UNORGANIZED DATA
    [HideInInspector] public Rigidbody rb;

    // PLAYER
    Player playerScript;


    [Header("Important")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform body;
    [SerializeField] PlayerCollisions playerCollisions;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerCollisions.onCollisionStay += OnCollisionStay;
        //playerCollisions.onCollisionEnter += OnCollisionEnter;
        playerCollisions.onCollisionExit += OnCollisionExit;

        readyToJump = true;
        moveForce = walkSpeed;
    }

    private void Update()
    {
        GroundCheck();
        SpeedControl();

        if (isGrounded && airTime > 0f) OnLanded();

        if (!isGrounded)
        {
            airTime += Time.deltaTime;
        }

        wasGrounded = airTime < 0.01f;

        body.rotation = new Quaternion(0, Camera.main.transform.rotation.y, 0, Camera.main.transform.rotation.w).normalized;


        DebugHUD.instance.SetSpeed(rb.velocity.magnitude);
    }

    void GroundCheck()
    {
        if (Physics.SphereCast(transform.position,0.1f,Vector3.down, out ground, playerHeight * 0.5f, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
        {
            groundAngle = Vector3.Angle(Vector3.up, ground.normal);
            DebugHUD.instance.SetDebugText("ground angle : " + groundAngle.ToString("0.0"));

            if (!isGrounded) rb.drag = oldDrag;
            isGrounded = true;
            return;
        }
        if (isGrounded) oldDrag = rb.drag;
        isGrounded = false;
        rb.drag = 0;
    }

    private void FixedUpdate()
    {
        Move();

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == wallobj)
        {
            wallobj = null;
            ignoreMovement = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // if we are colliding with something vertical
        if (collision.gameObject.layer == StaticUtilities.groundLayer)
        {
            foreach (var c in collision.contacts)
            {
                if (Vector3.Dot(c.normal, moveDirection) < -0.9f)
                {
                    Debug.Log(Vector3.Dot(c.normal, moveDirection));
                    wallobj = collision.gameObject;
                    break;
                }
                else
                {
                    wallobj = null;
                }
            }
        }
    }

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        this.actions = actions;
        playerScript = (Player)sender;

        // Movement
        actions.Locomotion.Move.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            this.input = Vector3.right * input.x + Vector3.forward * input.y;
            UpdateMoveSpeed();
        };

        actions.Locomotion.Move.canceled += ctx => 
        {
            input = Vector3.zero;
        };

        // Run
        actions.Locomotion.Run.started += ctx =>
        {
            isRunning = true;
            UpdateMoveSpeed();
        };

        actions.Locomotion.Run.canceled += ctx =>
        {
            isRunning = false;
            UpdateMoveSpeed();
        };

        // Jump
        actions.Locomotion.Jump.started += ctx =>
        {
            if (CanJump()) Jump();
        };

        //EnableLocomotion();
    }

    void Move()
    {

        if (groundAngle > maxSlopeAngle)
        {
            // slide down
            moveDirection = Vector3.ProjectOnPlane(Vector3.down, ground.normal);
            rb.AddForce(moveDirection * rb.mass * 10f, ForceMode.Force);
            return;
        }

        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0f;
        
        moveDirection = StaticUtilities.GetCameraDir() * input.z + cameraRight * input.x;
        if (isGrounded) moveDirection = Vector3.ProjectOnPlane(moveDirection, ground.normal);

        // move
        if (!ignoreMovement) rb.AddForce(moveDirection * moveForce * rb.mass * 10f, ForceMode.Force);
    }

    public bool CanJump()
    {
        return (isGrounded || wasGrounded) && readyToJump;
    }

    void Jump()
    {
        // prevent spamming
        readyToJump = false;
        isJumping = true;

        // stop any up/down movement
        rb.velocity = Vector3.right * rb.velocity.x + Vector3.forward * rb.velocity.z;

        // jump
        rb.AddForce(Vector3.up * jumpForce * rb.mass, ForceMode.Impulse);

        if (wallobj)
        {
            ignoreMovement = true;
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void SpeedControl()
    {
        float moveSpeed = isRunning ? input.z == 1 ? runSpeed : runSpeedLR : walkSpeed;
        if (isGrounded)
        {
            if (rb.velocity.sqrMagnitude > (moveSpeed * moveSpeed))
            {
                ignoreMovement = true;
                return;
            }
        }
        else
        {
            Vector3 xyVel = Vector3.right * rb.velocity.x + Vector3.forward * rb.velocity.z;
            if (xyVel.sqrMagnitude > (airborneXYSpeed * airborneXYSpeed))
            {
                ignoreMovement = true;
                return;
            }
        }

        if (!wallobj && !isJumping) ignoreMovement = false;
    }

    void UpdateMoveSpeed()
    {
        if (isGrounded && !isRunning) moveForce = walkForce;
        else if (isGrounded && isRunning) moveForce = runForce;
        else if (!isGrounded) moveForce = airborneForce;
    }

    Vector3 CalculateBodyRotation()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 viewDir = transform.position - Vector3.right * cameraPosition.x + Vector3.up * transform.position.y + Vector3.forward * cameraPosition.z;
        return viewDir.normalized;
    }

    public void RecalculateBodyRotation()
    {
        orientation.forward = CalculateBodyRotation();
    }

    void OnLanded()
    {
        wasGrounded = true;
        isJumping = false;
        //ignoreMovement = false;
        if (jumpCooldown > 0) Invoke(nameof(ResetJump), jumpCooldown);
        else ResetJump();
        airTime = 0;
    }

    public bool IsMoving() => input != Vector3.zero;
    
    public Transform GetBody() => body;
    public Vector3 GetMoveDirection() => moveDirection;

    // Controls Toggles
    public void EnableLocomotion() => actions.Locomotion.Enable();
    public void DisableLocomotion() => actions.Locomotion.Disable();*/
