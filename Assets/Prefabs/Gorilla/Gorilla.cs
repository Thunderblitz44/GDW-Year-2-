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
  
    void Start()
    {
        EnableAI();
    
        RangedAttack = GetComponentsInChildren<MeleeHitBox>(true);
        foreach (var trigger in RangedAttack)
        {
            trigger.damage = RangedAttackDamage;
            trigger.knockback = RangedKnockback;
        }
       
        int randomDeathType = Random.Range(0, 3);
        DeathType = randomDeathType;
        animator.SetInteger("deathType", randomDeathType);
        Invoke("Doom", 120f);

    }


    protected override void Update()
    {
        base.Update();
        Vector3 headPosition = LevelManager.PlayerTransform.position;
        
        if (isEnabled)
        {
            HeadTarget.transform.position = headPosition;
    
            if (agent && agent.isOnNavMesh)
            {
                agent.SetDestination(headPosition);
            }
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
         
            inAttackRange = true;
            animator.SetBool("inAttackRange", inAttackRange);
            
        }
    }

  
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
       
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

