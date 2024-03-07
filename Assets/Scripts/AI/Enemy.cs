using UnityEngine;
using UnityEngine.AI;

public class Enemy : DamageableEntity
{
    protected Transform target;
    protected NavMeshAgent agent;
    [SerializeField] float slowUpdateInterval = 0.2f;
    float updateTimer;
    protected bool updateTargetOnDamaged = true;

    protected Animator animator;
    protected SkinnedMeshRenderer skinnedMeshRenderer;
    float flashDuration = 0.2f; 
    float flashTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        
        animator = GetComponent<Animator>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

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
        
        
        if (flashTimer > 0 && skinnedMeshRenderer)
        {
            flashTimer -= Time.deltaTime;
            float flashIntensity = Mathf.Lerp(0f, 1f, flashTimer / flashDuration);
            skinnedMeshRenderer.material.SetFloat("_flash", flashIntensity);
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
        flashTimer = flashDuration;
        if (updateTargetOnDamaged) target = LevelManager.PlayerTransform;
    }
}
