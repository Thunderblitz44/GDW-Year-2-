using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class Bat : Enemy
{
   private bool isEnabled = false;
   public VisualEffect vfxGraph;
   [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    public CapsuleCollider attackTrigger;
    public GameObject HeadTarget;
    MeleeHitBox[] RangedAttack;
    private bool inAttackRange;
    public int RangedAttackDamage;
    public Vector2 RangedKnockback;
    private float xSpeed;
    private float zSpeed;
    Vector3 localVelocity;
    // Start is called before the first frame update
    void Start()
    {
        EnableAI();
      GetComponentInChildren<MagicBullet>().Initialize(projectile, this);
    }
    
    protected override void Update()
    {
        base.Update();
        Vector3 headPosition = LevelManager.PlayerTransform.position;
        
        if (isEnabled)
        {
            HeadTarget.transform.position = headPosition;
            if (agent) agent.SetDestination(headPosition);
         
        }

        
        float smoothingFactor = 0.1f;

        if (agent) localVelocity = transform.InverseTransformDirection(agent.velocity.normalized);
        else localVelocity = Vector3.zero;

        xSpeed = Mathf.Lerp(xSpeed, localVelocity.x, smoothingFactor);
        zSpeed = Mathf.Lerp(zSpeed, localVelocity.z, smoothingFactor);
      

        
    }

    // temporary until we get a death animation
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
    
    public void DeathBurst()
    {
      
        vfxGraph.SendEvent("death");
    }

    public void Spawn()
    {
        
    }
}
