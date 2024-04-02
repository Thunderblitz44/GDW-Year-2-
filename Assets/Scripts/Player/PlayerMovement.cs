using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.Events;

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
    float oldMoveSpeed = 0f;
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
    [Space(10f)]

    // Smoothing
    float stickFactor = 2f;

    // INPUT
    Vector3 input;
    Vector3 moveDirection;
    ActionMap actions;

    // PLAYER STUFF
    public Transform Body { get; private set; }
    public Rigidbody Rb { get; private set; }

    [HideInInspector] public bool IsDead;
    public event UnityAction OnPlayerDeath;
  
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
    }

    private void Update()
    {
        if (Physics.Raycast(groundCheck.position, Vector3.down, out ground, groundRaycastDist, groundLayer, QueryTriggerInteraction.Ignore))
        {
            groundAngle = Vector3.Angle(Vector3.up, ground.normal);

            if (!IsGrounded) Debug.Log("on landed");

            IsGrounded = true;
            if (input == Vector3.zero) Rb.useGravity = true;
            else Rb.useGravity = false;
        } 
        else if (!stepClimbing)
        {
            IsGrounded = false;
            Rb.useGravity = true;
        }

        // stick to ground
        stickFactor = Mathf.Lerp(0.5f, 2f, ground.distance/groundRaycastDist);
        //Debug.LogFormat("{0} - {1}", ground.distance, stickFactor);

        if (stickFactor > 1) Debug.Log(stickFactor);

        // can jump test
        if (IsGrounded && (jumpCooldownTimer += Time.deltaTime) >= jumpCooldown) canJump = true;
        if (jumped && IsGrounded && canJump) jumped = false;

        // rotate body
        Body.rotation = Quaternion.LookRotation(StaticUtilities.GetCameraDir());

    }

    private void FixedUpdate()
    {
        // do move
        if (IsGrounded && Rb.velocity.magnitude < MoveSpeed)
        {
            moveDirection = StaticUtilities.GetCameraDir() * input.z + StaticUtilities.HorizontalizeVector(Camera.main.transform.right) * input.x;
            if (IsGrounded && groundAngle < maxSlopeAngle)
            {
                moveDirection = Vector3.ProjectOnPlane(moveDirection, ground.normal);
            }
            Rb.velocity += MoveAcceleration * Time.fixedDeltaTime * moveDirection - (ground.normal*stickFactor);
            oldMoveSpeed = MoveSpeed;
        }
        else if (!IsGrounded)
        {
            moveDirection = StaticUtilities.GetCameraDir() * input.z + StaticUtilities.HorizontalizeVector(Camera.main.transform.right) * input.x;
            Vector3 newVel = Rb.velocity + MoveAcceleration * Time.fixedDeltaTime * moveDirection;
            float speed = StaticUtilities.HorizontalizeVector(newVel).magnitude;
            if (speed < oldMoveSpeed)
            {
                Rb.velocity = newVel;
            }

            if (speed < MoveSpeed) oldMoveSpeed = MoveSpeed;
        }

        // Steps
        RaycastHit hitLower;
        if (Physics.Raycast(groundCheck.position, moveDirection, out hitLower, 0.25f))
        {
            if (Vector3.Dot(hitLower.normal, moveDirection) < -0.7f && !Physics.Raycast(kneeClearanceCheck.position, moveDirection, 0.4f))
            {
                stepClimbing = true;
                Rb.position += Vector3.up * 0.08f;
            }
        }
        else if (stepClimbing) stepClimbing = false;

        Debug.DrawRay(transform.position, Rb.velocity, Color.green, Time.fixedDeltaTime);
        Debug.DrawRay(transform.position, moveDirection * 2, Color.red, Time.fixedDeltaTime);
        Debug.DrawRay(ground.point, ground.normal/2, Color.red, Time.fixedDeltaTime);
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

            //if (input != Vector2.zero && !IsGrounded) Rb.drag = 1f;
            //else Rb.drag = 0;
        };

        actions.Locomotion.Move.canceled += ctx =>
        {
            input = Vector3.zero;
            isRunning = false;
        };

        // Run
        actions.Locomotion.Run.performed += ctx =>
        {
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

    public void GroundTrail()
    {
        
    }

    public void Death()
    {
        IsDead = true;
        // Invoke the event if it's not null
        OnPlayerDeath?.Invoke();
    }
}
