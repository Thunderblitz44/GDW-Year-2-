using UnityEngine;
using UnityEngine.VFX;
public class Gorilla : Enemy
{
    private bool isEnabled = false;
   
    public CapsuleCollider attackTrigger;
    public GameObject HeadTarget;
    MeleeHitBox[] RangedAttack;
    private bool inAttackRange;
    public int RangedAttackDamage;
    public Vector2 RangedKnockback;
    private float xSpeed;
    private float zSpeed;
    Vector3 localVelocity;
    public ParticleSystem DustSystemRight;
    public ParticleSystem DustSystemLeft;
    public VisualEffect vfxGraph;
    private int AttackType;
    private int DeathType;
    // Start is called before the first frame update
    void Start()
    {
        EnableAI();
    
        RangedAttack = GetComponentsInChildren<MeleeHitBox>(true);
        foreach (var trigger in RangedAttack)
        {
            trigger.damage = RangedAttackDamage;
            trigger.knockback = RangedKnockback;
        }
       
        int randomDeathType = Random.Range(0, 2);
        DeathType = randomDeathType;
        animator.SetInteger("deathType", randomDeathType);
    }


    protected override void Update()
    {
        base.Update();
        Vector3 headPosition = LevelManager.PlayerTransform.position;
        
        if (isEnabled)
        {
            HeadTarget.transform.position = headPosition;
            if (agent) agent.SetDestination(headPosition);
            animator.SetBool("IsAttacking", inAttackRange);
        }

        
        float smoothingFactor = 0.1f;

        if (agent) localVelocity = transform.InverseTransformDirection(agent.velocity.normalized);
        else localVelocity = Vector3.zero;

        xSpeed = Mathf.Lerp(xSpeed, localVelocity.x, smoothingFactor);
        zSpeed = Mathf.Lerp(zSpeed, localVelocity.z, smoothingFactor);
      
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
    
        
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void EnableAI()
    {
        agent.enabled = true;
        isEnabled = true;
      
        HeadTarget.SetActive(true);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player entered the attack trigger, set inAttackRange to true
            inAttackRange = true;
            animator.SetBool("inAttackRange", inAttackRange);
            // You can also trigger an attack animation here if needed
            // GolemKnightAnimator.SetTrigger("AttackTrigger");
        }
    }

    // Called when another collider exits the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player exited the attack trigger, set inAttackRange to false
            inAttackRange = false;
            animator.SetBool("inAttackRange", inAttackRange);
        }
    }
    
    public void DustLeft()
    {
        DustSystemLeft.Emit(6);
    }
    
    public void DustRight()
    {
        DustSystemRight.Emit(6);
    }
    public void DeathBurst()
    {
        vfxGraph.SendEvent("death");
    }

    public void ChooseNextAttack()
    {
        AttackType = Random.Range(0, 3);
        animator.SetInteger("AttackType", AttackType);
    }
}

