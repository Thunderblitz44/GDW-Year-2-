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
    void Awake()
    {
        SharkAgent = GetComponent<NavMeshAgent>();
            SharkAnimator = GetComponent<Animator>();
            
    }

    // Update is called once per frame
    void Update()
    {
         
        GameObject lockOn = GameObject.FindGameObjectWithTag("HeadTag");
        Vector3 headPosition = lockOn.transform.position;

        
        if (isEnabled)
        {
            // Lerp the y position of SharkObject towards the y position of HeadTarget
         
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

    public System.Collections.IEnumerator LockOnCoroutine()
    {
        float elapsedTime = 0f;
        float duration = 3f; // Set the duration of the lock-on effect

        while (elapsedTime < duration)
        {
            SharkObject.position = new Vector3(
                SharkObject.position.x,
                Mathf.Lerp(SharkObject.position.y, HeadTarget.transform.position.y, Time.deltaTime * lerpSpeed),
                SharkObject.position.z
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset or do any other cleanup after the lock-on effect
    }
}
