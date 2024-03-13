using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class MantisController : Enemy
{
    private bool isEnabled = false;
    public CapsuleCollider attackTrigger;
    public GameObject HeadTarget;
    public MeleeHitBox[] RangedAttack;
    private bool inAttackRange;
    public int RangedAttackDamage;
    public Vector2 RangedKnockback;

    private VisualEffect teleportEffect; // Reference to the teleport Visual Effect Graph

    void Start()
    {
        RangedAttack = GetComponentsInChildren<MeleeHitBox>(true);
        foreach (var trigger in RangedAttack)
        {
            trigger.damage = RangedAttackDamage;
            trigger.knockback = RangedKnockback;
        }

        // Get the teleport Visual Effect Graph component
        teleportEffect = GetComponentInChildren<VisualEffect>();
        if (teleportEffect == null)
        {
            Debug.LogError("Teleport Visual Effect Graph component not found!");
        }
    }

    // Method to teleport after a delay
    public void Teleport()
    {
        
        PlayTeleportEffect();
        StartCoroutine(WarpToPlayerAfterDelay(0.5f)); 
    }

    // Coroutine to warp the agent to the player position after a delay
    private IEnumerator WarpToPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        
        // Enable AI and warp the agent to the player position
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
            animator.SetBool("IsAttacking", inAttackRange);
        }
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

    // Method to play the teleport Visual Effect Graph event
    private void PlayTeleportEffect()
    {
        
            teleportEffect.SendEvent("Teleport");
       
        
            
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}