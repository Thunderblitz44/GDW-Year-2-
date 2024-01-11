using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemKnightAnimatorController : MonoBehaviour
{
    private NavMeshAgent GolemKnightAgent;
    private Animator GolemKnightAnimator;

    public float maxSpeed = 5f; // Maximum speed of the AI
    public float minSpeed = 1f; // Minimum speed the AI can have
    public float minDistance = 2f; // Minimum distance at which the AI starts reducing speed

    // Start is called before the first frame update
    void Start()
    {
        GolemKnightAnimator = GetComponent<Animator>();
        GolemKnightAgent = GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    { 
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 targetPosition = player.transform.position;

        GolemKnightAgent.SetDestination(targetPosition);

        // Calculate distance between AI and target
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // Gradually decrease speed based on distance
        if (distanceToTarget < minDistance)
        {
            float reducedSpeed = Mathf.Lerp(maxSpeed, minSpeed, distanceToTarget / minDistance);
            GolemKnightAgent.speed = reducedSpeed;
           
        }
        else
        {
            GolemKnightAgent.speed = maxSpeed; // Maintain maximum speed if far from the target
        }

        // Update animator with normalized speed
        GolemKnightAnimator.SetFloat("ZSpeed", GolemKnightAgent.velocity.magnitude);
    }
}


