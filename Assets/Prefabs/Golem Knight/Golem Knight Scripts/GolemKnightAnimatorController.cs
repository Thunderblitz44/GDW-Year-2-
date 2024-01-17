using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemKnightAnimatorController : MonoBehaviour
{
    private NavMeshAgent GolemKnightAgent;
    private Animator GolemKnightAnimator;
    private bool isEnabled = false;
    public float maxSpeed = 5f; // Maximum speed of the AI
    public float minSpeed = 1f; // Minimum speed the AI can have
    public float minDistance = 2f; // Minimum distance at which the AI starts reducing speed

    public Transform HeadTarget;
    // Start is called before the first frame update
    void Start()
    {
        GolemKnightAnimator = GetComponent<Animator>();
        GolemKnightAgent = GetComponentInParent<NavMeshAgent>();
        HeadTarget = HeadTarget.GetComponent<Transform>();
        GolemKnightAgent.enabled = false;
    }

    
    // Update is called once per frame
    public void Update()
    { 
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject lockOn = GameObject.FindGameObjectWithTag("HeadTag");
        Vector3 headPosition = lockOn.transform.position;
        Vector3 targetPosition = player.transform.position;
        HeadTarget.transform.position = headPosition;
        
    //    GameObject playerHead = GameObject.FindGameObjectWithTag("HeadTag");
        
        if (isEnabled)
        {
            GolemKnightAgent.SetDestination(targetPosition);
        
        }
     

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
//Enables the ai on a delay
    private void EnableAI()
    {
        GolemKnightAgent.enabled = true;
        isEnabled = true;
      
        

    }
}


