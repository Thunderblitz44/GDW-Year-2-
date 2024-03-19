using UnityEngine;
using UnityEngine.VFX;

public class GolemKnight : Enemy
{
    // attack
    [SerializeField] int attackDamage = 25;
    //[SerializeField] float attackCooldown = 1.5f;
    //[SerializeField] float attackDelay = 0.5f;
    //float attackTimer;
    //float attackCooldownTimer;
    //bool attack;
    [SerializeField] GameObject HeadTarget;
    AttackTrigger trigger;
    MeleeHitBox sword;
    private float DeathType;
    private float xSpeed;
    private float ySpeed;
    private float zSpeed;
     public VisualEffect vfxGraph;
     
     public ParticleSystem DustSystemRight;
     public ParticleSystem DustSystemLeft;
    Vector3 localVelocity;
    protected override void Awake()
    {
       
        base.Awake();

        sword = transform.GetComponentInChildren<MeleeHitBox>(true);
        sword.damage = attackDamage;
        int randomDeathType = Random.Range(0, 2);
        DeathType = randomDeathType;
        animator.SetInteger("deathType", randomDeathType);
       
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
        if (!target) return;

        HeadTarget.transform.position = target.position;

        float smoothingFactor = 0.1f;

        if (agent) localVelocity = transform.InverseTransformDirection(agent.velocity.normalized);
        else localVelocity = Vector3.zero;

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
        //attack = true;
        animator.SetBool("CanAttack", true);
    }

    void OnAttackTriggerExit(Collider other)
    {
        //attack = false;
        //attackTimer = 0f;
        animator.SetBool("CanAttack", false);
    }

    public void EnableAI()
    {
        if (agent) agent.enabled = true;
        HeadTarget.SetActive(true);
    }

    public void DisableAI()
    {
        if (agent) agent.enabled = false;
        HeadTarget.SetActive(false);
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
        //Debug.Log("1");
        sword.gameObject.SetActive(true);
    }

    public void DisableAttackR()
    {
        sword.gameObject.SetActive(false);
        //Debug.Log("2");
    }
    public void ReadyAttackL()
    {
       // Debug.Log("3");
        sword.gameObject.SetActive(true);
    }

    public void DisableAttackL()
    {
        sword.gameObject.SetActive(false);
        //Debug.Log("4");
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void DeathBurst()
    {
        LineRenderer[] lineRenderers = GetComponentsInChildren<LineRenderer>();

        
        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            Destroy(lineRenderer.gameObject);
        }
     vfxGraph.SendEvent("death");
    }
    
    public void DustLeft()
    {
        DustSystemLeft.Emit(6);
    }
    
    public void DustRight()
    {
        DustSystemRight.Emit(6);
    }
}
