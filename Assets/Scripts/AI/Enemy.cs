using UnityEngine;
using UnityEngine.AI;

public class Enemy : DamageableEntity
{
    internal Transform target;
    internal NavMeshAgent agent;
    [SerializeField] float slowUpdateInterval = 0.2f;
    float updateTimer;
    
    [SerializeField] internal Animator animator;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        
    }

    protected virtual void Update()
    {
        // timer to recalculate navmesh agent
        updateTimer += Time.deltaTime;
        if (updateTimer >= slowUpdateInterval) SlowUpdate();
    }

    protected virtual void SlowUpdate()
    {
        updateTimer = 0;
        if (!target || !agent || !agent.isActiveAndEnabled || !LevelManager.Instance.NavMesh || !LevelManager.Instance.NavMesh.isActiveAndEnabled) return;
        agent.SetDestination(target.position);
    }

    public virtual void SetFollowTarget(Transform target)
    {
        this.target = target;
    }

    public bool IsAnimationPlaying(int layer)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1;
    }
}
