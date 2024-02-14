using UnityEngine;

public class GolemKnight : Enemy
{
    // attack
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float attackDelay = 0.5f;
    float attackTimer;
    float attackCooldownTimer;
    bool attack;
    [SerializeField] GameObject HeadTarget;
    AttackTrigger trigger;
    MeleeHitBox sword;

    private float xSpeed;
    private float ySpeed;
    private float zSpeed;

    protected override void Awake()
    {
        base.Awake();

        sword = transform.GetComponentInChildren<MeleeHitBox>(true);
        sword.damage = attackDamage;

        trigger = transform.GetComponentInChildren<AttackTrigger>();
        if (trigger)
        {
            trigger.onTriggerEnter += OnAttackTriggerEnter;
            trigger.onTriggerExit += OnAttackTriggerExit;
        }
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

    void OnAttackTriggerEnter(Collider other)
    {
        attack = true;
        animator.SetBool("CanAttack", true);
    }

    void OnAttackTriggerExit(Collider other)
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
        agent.speed = 2;
    }

    public void ReadyAttackR()
    {
        Debug.Log("1");
        sword.gameObject.SetActive(true);
    }

    public void DisableAttackR()
    {
        sword.gameObject.SetActive(false);
        Debug.Log("2");
    }
    public void ReadyAttackL()
    {
        Debug.Log("3");
        sword.gameObject.SetActive(true);
    }

    public void DisableAttackL()
    {
        sword.gameObject.SetActive(false);
        Debug.Log("4");
        
    }
}
