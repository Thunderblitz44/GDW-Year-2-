using UnityEngine;
using UnityEngine.AI;

public class Enemy : DamageableEntity
{
    protected Transform target;
    protected NavMeshAgent agent;
    [SerializeField] float slowUpdateInterval = 0.2f;
    protected bool updateTargetOnDamaged = true;
    float updateTimer;

    protected Animator animator;
    protected SkinnedMeshRenderer skinnedMeshRenderer;
    public float flashTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        
        animator = GetComponent<Animator>();
        if (!animator) animator = GetComponentInChildren<Animator>();
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
        
        if (skinnedMeshRenderer && flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            float flashIntensity = Mathf.Lerp(0f, 1f, flashTimer / StaticUtilities.damageFlashDuration);
            skinnedMeshRenderer.material.SetFloat("_flash", flashIntensity);
        }
    }

    protected override void OnHealthZeroed()
    {
        Destroy(agent);
        base.OnHealthZeroed();
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
        flashTimer = StaticUtilities.damageFlashDuration;
        if (updateTargetOnDamaged) target = LevelManager.PlayerTransform;
    }
}
