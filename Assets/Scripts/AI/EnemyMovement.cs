using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy), typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    EnemyAttack attackHandler;
    [SerializeField] float destinationRecalculationInterval = 0.2f;
    float updateTimer;

    public Action onAttackDistanceReached;
    public Action onTargetFled;

    bool targetReached;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        attackHandler = GetComponent<EnemyAttack>();
        agent.stoppingDistance = attackHandler.GetRange() / 2;
    }

    private void OnDestroy()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!agent) return;

        // timer to recalculate navmesh agent
        updateTimer += Time.deltaTime;
        if (updateTimer < destinationRecalculationInterval)
        {
            return;
        }

        SlowUpdate();
    }   

    void SlowUpdate()
    {
        updateTimer = 0;
        if (!target) return;

        // go to target
        agent.SetDestination(target.position);

        float distance = Vector3.Distance(transform.position, target.position);

        // reached attacking distance
        if (distance < (attackHandler ? attackHandler.GetRange() : 2) && !targetReached)
        {
            targetReached = true;
            onAttackDistanceReached?.Invoke();
        }
        // target is fleeing. Can we attack? Is the target outside of our range?
        else if (agent.isStopped && targetReached && 
            (attackHandler ? !attackHandler.isAttacking : true) && 
            distance >= (attackHandler ? attackHandler.GetRange() : 2))
        {
            targetReached = false;
            onTargetFled?.Invoke();
        }
    }

    public void SetFollowTarget(Transform target)
    {
        this.target = target;
    }
}
