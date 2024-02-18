using System.Collections;
using UnityEngine;

public class SharkBoss : Enemy
{
    
   
    private bool isEnabled = false;
    public float maxSpeed = 5f; 
    public float minSpeed = 1f; 
    public float minDistance = 2f; 
    public Transform SharkObject;
    public GameObject HeadTarget;
    public float lerpSpeed = 1f;

    protected override void Awake()
    {
        base.Awake();
        EnableAI();
    }

    protected override void Update()
    {
        base.Update();
        GameObject lockOn = GameObject.FindGameObjectWithTag("HeadTag");
        Vector3 headPosition = lockOn.transform.position;

        if (isEnabled)
        {
        
            float lerpFactor = lerpSpeed * Time.deltaTime;

           
            SharkObject.position = new Vector3(
                SharkObject.position.x,
                Mathf.Lerp(SharkObject.position.y, headPosition.y, lerpFactor),
                SharkObject.position.z
            );

          
            agent.SetDestination(headPosition);
        }

    }
    private void EnableAI()
    {
        agent.enabled = true;
        isEnabled = true;
      
        HeadTarget.SetActive(true);

    } 
    
    public IEnumerator LockOnCoroutine()
    {
        float elapsedTime = 0f;
        float duration = 3f; 

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

 
    }
    
}
