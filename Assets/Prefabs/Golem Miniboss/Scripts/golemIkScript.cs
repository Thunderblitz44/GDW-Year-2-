using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class golemIkScript : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer = default;
    [SerializeField] private Transform body = default;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float stepDistance = 4f;
    [SerializeField] private float stepLength = 4f;
    [SerializeField] private float stepHeight = 1f;
    [SerializeField] private Vector3 footOffset = default;
    public bool footDelay = true;
    private float footSpacing;
    private Vector3 oldPosition, currentPosition, newPosition;
    private Vector3 oldNormal, currentNormal, newNormal;
    private float lerp;
    // Access the ControllerScript
    public legController controllerScript;

    private void Start()
    {
        if (controllerScript == null)
        {
            controllerScript = FindObjectOfType<legController>();
        }
        InitializeGolem();
    }

    private void InitializeGolem()
    {
        
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1f;
    }

    private void Update()
    {

       
        HandleGolemMovement();
       
    }
    private void HandleGolemMovement()
    {
       
        transform.position = currentPosition;
      
        transform.up = currentNormal;

       
        Ray ray = CreateFootRay();

        if (Physics.Raycast(ray, out RaycastHit info, 10f, terrainLayer.value))
        {
           
            HandleStep(info);
        }

        if (lerp < 1f)
        {
          
            InterpolateGolemPositionAndRotation();
        }
        else
        {
         
            FinishStep();
           
        }
    }

    private Ray CreateFootRay()
    {
       
        return new Ray(body.position + (body.right * footSpacing), Vector3.down);

    }

    public void HandleStep(RaycastHit info)
    {
       
        if (footDelay == true)
        {
            if (Vector3.Distance(newPosition, info.point) > stepDistance && lerp >= 1f)
            {
               
                lerp = 0f;
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = info.normal;
            
            }
        }

        else
        {

        }
    }

    private void InterpolateGolemPositionAndRotation()
    {
    

        Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
        tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

        
        currentPosition = tempPosition;
        currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);

       
        lerp += Time.deltaTime * speed;
       
    }
  
    public void FinishStep()
    {
  
        oldPosition = newPosition;
        oldNormal = newNormal;


        if (footDelay == true)
        {
            controllerScript.SetActiveScript();
        }
    }


    // private void OnDrawGizmos()
    // {

    //     DrawGizmoForNewPosition();
    //  }

    // private void DrawGizmoForNewPosition()
    // {
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(newPosition, 0.5f);
    // }



}
