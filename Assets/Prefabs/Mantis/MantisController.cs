using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class MantisController : Enemy
{
    private bool isEnabled = true;
    public CapsuleCollider attackTrigger;
    public GameObject HeadTarget;
    public MeleeHitBox[] RangedAttack;
    private bool inAttackRange;
    public int RangedAttackDamage;
    public Vector2 RangedKnockback;
    public VisualEffect vfxGraph;
    private VisualEffect teleportEffect; // Reference to the teleport Visual Effect Graph

    void Start()
    {
        RangedAttack = GetComponentsInChildren<MeleeHitBox>(true);
        foreach (var trigger in RangedAttack)
        {
            trigger.damage = RangedAttackDamage;
            trigger.knockback = RangedKnockback;
        }

        
        teleportEffect = GetComponentInChildren<VisualEffect>();
        if (teleportEffect == null)
        {
            Debug.LogError("Teleport Visual Effect Graph component not found!");
        }
    }

 
    public void Teleport()
    {
        
        PlayTeleportEffect();
        StartCoroutine(WarpToPlayerAfterDelay(0.5f)); 
    }

    
    private IEnumerator WarpToPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 
        
        
        EnableAI();
        if (agent) agent.Warp(LevelManager.PlayerTransform.position);

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inAttackRange = false;
        }
    }

  
    private void PlayTeleportEffect()
    {
        
            teleportEffect.SendEvent("Teleport");
       
        
            
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    
    public void DeathBurst()
    {
        vfxGraph.SendEvent("death");
    }
}