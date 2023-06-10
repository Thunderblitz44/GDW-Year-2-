using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Zombie), typeof(Rigidbody))]
public class ZombieMovement : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;

    [SerializeField] float destinationRecalculationInterval = 0.2f;
    bool updateDestination;
    float updateTimer;

    public Action onTargetReached;
    public Action onTargetFled;

    bool targetReached;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnDestroy()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // timer to recalculate navmesh agent
        updateTimer += Time.deltaTime;
        if (updateTimer >= destinationRecalculationInterval && !updateDestination)
        {
            updateTimer = 0;
            updateDestination = true;
        }


        if (target && updateDestination)
        {
            updateDestination = false;
            agent.SetDestination(target.position);
        }

        if (target && agent.isStopped && !targetReached)
        {
            targetReached = true;
            onTargetReached?.Invoke();
        }
        else if (target && !agent.isStopped && targetReached)
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
