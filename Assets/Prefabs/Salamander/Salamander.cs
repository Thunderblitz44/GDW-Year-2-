using UnityEngine;

public class Salamander : Enemy
{
    private bool isEnabled = false;
  
    public CapsuleCollider attackTrigger;
    public GameObject HeadTarget;
    private float xSpeed;
    private float zSpeed;
    private bool inAttackRange;
    public int RangedAttackDamage;
    public Vector2 RangedKnockback;
    Vector3 localVelocity;
    MeleeHitBox[] RangedAttack;
    // Start is called before the first frame update
    void Start()
    {
       
        RangedAttack = GetComponentsInChildren<MeleeHitBox>(true);
        foreach (var trigger in RangedAttack)
        {
            trigger.damage = RangedAttackDamage;
            trigger.knockback = RangedKnockback;
        }
         //  MantisAgent.enabled = false;
         EnableAI();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Vector3 headPosition = LevelManager.PlayerTransform.position;
        
        if (isEnabled)
        {
            HeadTarget.transform.position = headPosition;
            if (agent) agent.SetDestination(headPosition);
            //animator.SetBool("IsAttacking", inAttackRange);
        }

        float smoothingFactor = 0.1f;

        if (agent) localVelocity = transform.InverseTransformDirection(agent.velocity.normalized);
        else localVelocity = Vector3.zero;

        // Smooth the velocity components (remove the float keyword)
        xSpeed = Mathf.Lerp(xSpeed, localVelocity.x, smoothingFactor);
        zSpeed = Mathf.Lerp(zSpeed, localVelocity.z, smoothingFactor);
        // Set the velocity values in the animator
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
        
    }

    // temporary until we get a death animation
    protected override void OnHealthZeroed()
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
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
