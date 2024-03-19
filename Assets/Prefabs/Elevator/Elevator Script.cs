using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour
{
    public Transform objectToMove; 
    public Transform pointA; 
    public Transform pointB; 
    public float lerpSpeed = 1f; 

    private float lerpTime = 0f;
    private bool isMoving = false;

    void Start()
    {
        if (objectToMove == null)
        {
            Debug.LogError("No object to move assigned to the LerpingScript.");
            return;
        }
    }

    public void StartLerping()
    {
        if (!isMoving)
        {
            isMoving = true;
            StartCoroutine(LerpObject());
        }
    }

    private System.Collections.IEnumerator LerpObject()
    {
        while (isMoving)
        {
            float distanceToB = Vector3.Distance(objectToMove.position, pointB.position);
            float distanceToA = Vector3.Distance(objectToMove.position, pointA.position);

            if (distanceToB <= 0.05f) 
            {
                isMoving = false;
                lerpTime = 0f;
                objectToMove.position = pointB.position; 
                yield break;
            }

            lerpTime += Time.fixedDeltaTime * lerpSpeed;

         
            lerpTime = Mathf.Clamp01(lerpTime);

            
            objectToMove.position = Vector3.Lerp(pointA.position, pointB.position, lerpTime);

            yield return new WaitForFixedUpdate();
        }

    }
}
    

