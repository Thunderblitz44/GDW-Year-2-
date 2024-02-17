using UnityEngine;

public class golemIkScript : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer = default;
    [SerializeField] private Transform hint = default;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float stepDistance = 4f;
    [SerializeField] private float stepLength = 4f;
    [SerializeField] private float stepHeight = 1f;
    [SerializeField] private Vector3 footOffset = default;
    public bool footDelay = true;
    private Vector3 oldPosition, currentPosition, newPosition;
    private Vector3 oldNormal, currentNormal, newNormal;
    private float lerp;
    // Access the ControllerScript
    public legController controllerScript;
    public bool footOverride = false;
    private void Awake()
    {
        if (controllerScript == null)
        {
            controllerScript = FindObjectOfType<legController>();
        }

        // initializing
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1f;
    }

    private void Update()   
    {
        transform.position = currentPosition;
        transform.up = currentNormal;
        Vector3 pos = hint.position;
        Debug.DrawRay(pos, hint.up * 10f, Color.red, Time.deltaTime);
        if (Physics.Raycast(pos, hint.up, out RaycastHit info, 20f, terrainLayer.value, QueryTriggerInteraction.Ignore))
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

    public void HandleStep(RaycastHit info)
    {
        if (!footDelay && !footOverride) return;
     
        float dist = Vector3.Distance(newPosition, info.point);
        if (dist > stepDistance && lerp >= 1f)
        {
            lerp = 0f;
            int direction = hint.InverseTransformPoint(info.point).z > hint.InverseTransformPoint(newPosition).z ? 1 : -1;
            newPosition = info.point + (hint.forward * stepLength * direction) + footOffset;
            newNormal = info.normal;
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


    private void OnDrawGizmos()
     {
         DrawGizmoForNewPosition();
      }

     private void DrawGizmoForNewPosition()
     {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }
}
