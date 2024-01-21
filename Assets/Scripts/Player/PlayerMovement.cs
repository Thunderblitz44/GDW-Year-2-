using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IInputExpander
{
    // MOVEMENT
    [Header("Movement")]
    [SerializeField] float walkForce = 3f;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runForce = 3f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float runSpeedLR = 5f;
    [SerializeField] float airborneForce = 0.5f;
    [SerializeField] float airborneXYSpeed = 1f;
    //[SerializeField] AnimationCurve walkToRunCurve;
    //[SerializeField] float walkToRunSpeed = 1f;
    //[SerializeField] AnimationCurve runToWalkCurve;
    //[SerializeField] float runToWalkSpeed = 1f;
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
        if (Physics.SphereCast(transform.position,0.1f,Vector3.down, out ground, playerHeight * 0.5f, whatIsGround, QueryTriggerInteraction.Ignore))
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
        if (collision.gameObject.layer == StaticUtilities.groundLayer)
        {
            /*float dot = Vector3.Dot(collision.contacts[0].normal, StaticUtilities.GetCameraDir());
            Debug.Log(collision.contacts[0].normal);

            if (Vector3.Dot(collision.contacts[0].normal, moveDirection) < -0.9f)
            {
                airborneForce = temp;
                temp = 0;
            }*/
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        // if we are colliding with something vertical
        if (collision.gameObject.layer == StaticUtilities.groundLayer && IsJumping())
        {
            foreach (var c in collision.contacts)
            {
                if (Vector3.Dot(c.normal, moveDirection) < -0.9f)
                {
                    if (temp == 0)
                    {
                        temp = airborneForce;
                        airborneForce = 0;
                    }
                    break;
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

        EnableLocomotion();
    }

    void Move()
    {
        if (ignoreMovement) return;

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
        moveDirection = Vector3.ProjectOnPlane(moveDirection, ground.normal);

        // move
        if (isGrounded) rb.AddForce(moveDirection * moveForce * rb.mass * 10f, ForceMode.Force);
        else rb.AddForce(moveDirection * moveForce * rb.mass * 10f, ForceMode.Force);
    }

    public bool CanJump()
    {
        return (isGrounded || wasGrounded) && readyToJump;
    }

    void Jump()
    {
        // prevent spamming
        readyToJump = false;

        // stop any up/down movement
        rb.velocity = Vector3.right * rb.velocity.x + Vector3.forward * rb.velocity.z;

        // jump
        rb.AddForce(Vector3.up * jumpForce * rb.mass, ForceMode.Impulse);
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

        ignoreMovement = false;
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
        if (jumpCooldown > 0) Invoke(nameof(ResetJump), jumpCooldown);
        else ResetJump();
        Debug.Log("landed");
        airTime = 0;
    }

    public bool IsMoving() => input != Vector3.zero;
    
    public bool IsRunning() => isRunning;
    public bool IsGrounded() => isGrounded;
    public bool IsJumping() => readyToJump == false;
    public float GetMoveSpeed() => moveForce;
    public float GetAirTime() => airTime;
    public Transform GetOrientation() => orientation;
    public Rigidbody GetRigidbody() => rb;
    public Transform GetBody() => body;
    public Vector3 GetMoveDirection() => moveDirection;

    // Controls Toggles
    public void EnableLocomotion() => actions.Locomotion.Enable();
    public void DisableLocomotion() => actions.Locomotion.Disable();
}
