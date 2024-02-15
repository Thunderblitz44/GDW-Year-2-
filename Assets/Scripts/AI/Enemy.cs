using UnityEngine;
using UnityEngine.AI;

public class Enemy : DamageableEntity
{
    protected Transform target;
    protected NavMeshAgent agent;
    [SerializeField] float slowUpdateInterval = 0.2f;
    float updateTimer;
    
    [SerializeField] protected Animator animator;

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
        target = LevelManager.PlayerTransform;
    }
}
