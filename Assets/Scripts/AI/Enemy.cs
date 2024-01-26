using UnityEngine;
using UnityEngine.AI;

public class Enemy : DamageableEntity
{
    internal Transform target;
    internal NavMeshAgent agent;
    [SerializeField] float slowUpdateInterval = 0.2f;
    [SerializeField] AttackTrigger trigger;
    float updateTimer;
    
    [SerializeField] internal Animator animator;

    internal override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        if (trigger)
        {
            trigger.onTriggerEnter += OnTriggerEnter;
            trigger.onTriggerExit += OnTriggerExit;
        }
    }

    internal virtual void Update()
    {
        if (animator) animator.SetFloat("ZSpeed", agent.velocity.magnitude);

        // timer to recalculate navmesh agent
        updateTimer += Time.deltaTime;
        if (updateTimer >= slowUpdateInterval) SlowUpdate();
    }

    internal virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log(other); 
    }

    internal virtual void OnTriggerExit(Collider other)
    {
        Debug.Log(other);
    }

    internal virtual void SlowUpdate()
    {
        updateTimer = 0;
        if (!target || !agent || !LevelManager.Instance.NavMesh.isActiveAndEnabled) return;
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
