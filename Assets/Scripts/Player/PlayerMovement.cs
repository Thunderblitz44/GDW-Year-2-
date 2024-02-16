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
        /*
         if (IsGrounded)
        {
            Vector3 rotatedInput = StaticUtilities.GetCameraDir() * input.z + StaticUtilities.HorizontalizeVector(Camera.main.transform.right) * input.x;
            moveDirection = rotatedInput;
            if (IsGrounded && groundAngle < maxSlopeAngle)
            {
                moveDirection = Vector3.ProjectOnPlane(rotatedInput, ground.normal);
            }
            Rb.velocity += moveDirection * MoveAcceleration;

            // cap speed
            if (Rb.velocity.magnitude >= MoveSpeed)
            {
                Rb.velocity = moveDirection * MoveSpeed;
            }
        }
        else
        {
            moveDirection = StaticUtilities.GetCameraDir() * input.z + StaticUtilities.HorizontalizeVector(Camera.main.transform.right) * input.x;
            Rb.velocity += moveDirection * MoveAcceleration;

            // cap speed
            if (StaticUtilities.HorizontalizeVector(Rb.velocity).magnitude >= MoveSpeed)
            {
                Rb.velocity = moveDirection * MoveSpeed;
            }
        }
        DebugHUD.instance.SetSpeed(Rb.velocity.magnitude);
        */

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

        actions.Locomotion.Run.Enable();
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

    public void EnableLocomotion()
    {
        actions.Locomotion.Move.Enable();
        actions.Locomotion.Jump.Enable();
        actions.Locomotion.Dodge.Enable();
    }

    public void DisableLocomotion()
    {
        actions.Locomotion.Move.Disable();
        actions.Locomotion.Jump.Disable();
        actions.Locomotion.Dodge.Disable();
    }
}
