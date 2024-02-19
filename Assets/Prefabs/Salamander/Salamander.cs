using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Salamander : Enemy
{
   

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
       
        
         //  MantisAgent.enabled = false;
         EnableAI();
    }

    // Update is called once per frame
    void Update()
    {
             
        GameObject lockOn = GameObject.FindGameObjectWithTag("HeadTag");
        Vector3 headPosition = lockOn.transform.position;
        
        
        
            
        if (isEnabled)
        {
            HeadTarget.transform.position = headPosition;
            agent.SetDestination(headPosition);
            animator.SetBool("IsAttacking", inAttackRange);
        }

        
        
        
    }
    
    private void EnableAI()
    {
        agent.enabled = true;
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
