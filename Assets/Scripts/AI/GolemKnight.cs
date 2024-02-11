using UnityEngine;

public class GolemKnight : Enemy
{
    // attack
    [SerializeField] float attackDamage = 1f;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float attackDelay = 0.5f;
    float attackTimer;
    float attackCooldownTimer;
    bool attack;
    [SerializeField] GameObject HeadTarget;
  
    private float xSpeed;
    private float ySpeed;
    private float zSpeed;

    private void Start()
    {
  
    }

    protected override void Update()
    {
        base.Update();

        HeadTarget.transform.position = LevelManager.PlayerTransform.position;

        // attack cooldown + delay
        attackCooldownTimer += Time.deltaTime;
        if (attackCooldownTimer >= attackCooldown && attack && 
            (attackTimer += Time.deltaTime) >= attackDelay)
        {
            attackCooldownTimer = 0f;
           
        }
        float smoothingFactor = 0.1f;

        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity.normalized);

        // Smooth the velocity components (remove the float keyword)
        xSpeed = Mathf.Lerp(xSpeed, localVelocity.x, smoothingFactor);
        zSpeed = Mathf.Lerp(zSpeed, localVelocity.z, smoothingFactor);
        ySpeed = Mathf.Lerp(ySpeed, localVelocity.y, smoothingFactor);
        // Set the velocity values in the animator
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
        animator.SetFloat("YSpeed", ySpeed);

    }

    protected override void OnAttackTriggerEnter(Collider other)
    {
        attack = true;
        animator.SetBool("CanAttack", true);
    }

    protected override void OnAttackTriggerExit(Collider other)
    {
        attack = false;
        attackTimer = 0f;
        animator.SetBool("CanAttack", false);
    }

    public void EnableAI()
    {
        agent.enabled = true;
        TargetPlayer();
        HeadTarget.SetActive(true);
        //Debug.Log("Hi");
    }

    public void DisableAI()
    {
        agent.enabled = false;
        HeadTarget.SetActive(false);
        //target = LevelManager.PlayerTransform;
    }

    public void TargetPlayer()
    {
        target = LevelManager.PlayerTransform;
    }
    public void DisableAILocomotion()
    {
        agent.speed = 0.05f;
        agent.angularSpeed = 180;
    }
    public void EnableAILocomotion()
    {
      //  Debug.Log("Current agent speed: " + agent.speed); // Debugging line
        agent.speed = 2;
      //  Debug.Log("Agent speed set to 2"); // Debugging line
        
        //Debug.Log("dddddddddd");
    }

    public void ReadyAttackR()
    {
        
    }

    public void DisableAttackR()
    {
        
    }
    public void ReadyAttackL()
    {
        
    }

    public void DisableAttackL()
    {
        
    }
}
