using UnityEngine;
using UnityEngine.AI;

public class Enemy : DamageableEntity
{
    protected Transform target;
    protected NavMeshAgent agent;
    [SerializeField] float slowUpdateInterval = 0.2f;
    float updateTimer;
    
    [SerializeField] protected Animator animator;
    [SerializeField] protected SkinnedMeshRenderer skinnedMeshRenderer;
   protected float flashDuration = 0.2f; 
    protected float flashTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        
        agent = GetComponent<NavMeshAgent>();
        target = LevelManager.PlayerTransform;
        
    }

    protected virtual void Update()
    {
        if (!target) target = LevelManager.PlayerTransform;
        if (!target) return;

        // timer to recalculate navmesh agent
        updateTimer += Time.deltaTime;
        if (updateTimer >= slowUpdateInterval) SlowUpdate();
        
        
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            float flashIntensity = Mathf.Lerp(0f, 1f, flashTimer / flashDuration);
            skinnedMeshRenderer.material.SetFloat("_flash", flashIntensity);
            Debug.Log("hi");
        }
    }

    protected virtual void SlowUpdate()
    {
        updateTimer = 0;
        if (!agent || !agent.isActiveAndEnabled) return;
        agent.SetDestination(target.position);
    }

    public void SetFollowTarget(Transform target)
    {
        this.target = target;
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);
        
        // Trigger flash effect
        flashTimer += flashDuration;
        target = LevelManager.PlayerTransform;
    }
}
