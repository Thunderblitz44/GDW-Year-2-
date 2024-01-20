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
    public CapsuleCollider attackTrigger;
    public GameObject HeadTarget;

    private bool inAttackRange;
    // Start is called before the first frame update
    void Start()
    {
        GolemKnightAnimator = GetComponent<Animator>();
        GolemKnightAgent = GetComponentInParent<NavMeshAgent>();
       
        GolemKnightAgent.enabled = false;
        attackTrigger = GetComponent<CapsuleCollider>();
    }

    
    // Update is called once per frame
    public void Update()
    { 
       
        GameObject lockOn = GameObject.FindGameObjectWithTag("HeadTag");
        Vector3 headPosition = lockOn.transform.position;
       
      
        
    //    GameObject playerHead = GameObject.FindGameObjectWithTag("HeadTag");
        
        if (isEnabled)
        {
            HeadTarget.transform.position = headPosition;
            GolemKnightAgent.SetDestination(headPosition);
        GolemKnightAnimator.SetBool("IsAttacking", inAttackRange);
        }
     

        // Calculate distance between AI and target
        float distanceToTarget = Vector3.Distance(transform.position, headPosition);

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
      
        HeadTarget.SetActive(true);

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player entered the attack trigger, set inAttackRange to true
            inAttackRange = true;
            // You can also trigger an attack animation here if needed
            // GolemKnightAnimator.SetTrigger("AttackTrigger");
        }
    }

    // Called when another collider exits the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player exited the attack trigger, set inAttackRange to false
            inAttackRange = false;
        }
    }
}


