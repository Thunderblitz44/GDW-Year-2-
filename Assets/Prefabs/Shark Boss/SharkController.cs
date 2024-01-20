using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SharkController : MonoBehaviour
{
    
    
    private NavMeshAgent SharkAgent;
        private Animator SharkAnimator;
        private bool isEnabled = false;
        public float maxSpeed = 5f; // Maximum speed of the AI
        public float minSpeed = 1f; // Minimum speed the AI can have
        public float minDistance = 2f; // Minimum distance at which the AI starts reducing speed
        public Transform SharkObject;
        public GameObject HeadTarget;
        public float lerpSpeed = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        SharkAgent = GetComponent<NavMeshAgent>();
            SharkAnimator = GetComponent<Animator>();
            
            EnableAI();
    }

    // Update is called once per frame
    void Update()
    {
         
        GameObject lockOn = GameObject.FindGameObjectWithTag("HeadTag");
        Vector3 headPosition = lockOn.transform.position;

        
        if (isEnabled)
        {
            // Lerp the y position of SharkObject towards the y position of HeadTarget
            SharkObject.position = new Vector3(
                SharkObject.position.x,
                Mathf.Lerp(SharkObject.position.y, headPosition.y, Time.deltaTime * lerpSpeed),
                SharkObject.position.z
            );
            HeadTarget.transform.position = headPosition;
            SharkAgent.SetDestination(headPosition);
            
        }
        
    }
    private void EnableAI()
    {
        SharkAgent.enabled = true;
        isEnabled = true;
      
        HeadTarget.SetActive(true);

    }
}
