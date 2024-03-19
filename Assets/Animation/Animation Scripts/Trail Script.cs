using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrailScript : MonoBehaviour
{
  
   
   
    //jackalope
    public GameObject trailPrefab; 
    public bool isTrailActive2;
    public LayerMask groundLayer; 
    public float meshRefreshRate2 = 0.1f;
    public ParticleSystem DustSystemRight;
    public ParticleSystem DustSystemLeft;
    private Coroutine trailCoroutine;
    public ParticleSystem dirtTrail;
    
    public PlayerMovement PlayerMovement;
    
    //dodge
   public Transform positionToSpawn;
    public float activeTime = 2f;
    public bool isTrailActive;
    public float meshRefreshRate = 0.1f;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Material mat;
    public float meshDestroyDelay = 1f;
    private GameObject dodgeTrailObject;
    //other
    public ParticleSystem OnLanded;
   
    public float _alpha = 0.38f;
    void Start()
    {


    

    }
    private void FixedUpdate()
    {
   
    }

    private IEnumerator ActivateTrail2()
    {
       
      
        dirtTrail.Play();
        while (isTrailActive2)
        {
          
            RaycastHit hit;
            Vector3 raycastOrigin = transform.position + new Vector3(0, 3f, 0); 
            
            if (Physics.Raycast(raycastOrigin, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
               
                trailPrefab.transform.up = hit.normal;
                
                trailPrefab.transform.position = hit.point;
              
            }

            yield return null; 
        }

        isTrailActive2 = false; 
        
   
        dirtTrail.Stop();
        trailCoroutine = null;
    }

   IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0 && isTrailActive2 == false)
        {
            timeActive -= meshRefreshRate;

            GameObject gObj = new GameObject();
            gObj.transform.SetLocalPositionAndRotation(positionToSpawn.position,
                positionToSpawn.rotation * Quaternion.Euler(0, 180, 0));
            MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
            MeshFilter mf = gObj.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(mesh);

            mf.mesh = mesh;
            mr.material = mat;

            // Set initial alpha value using direct property access
            mr.material.SetFloat("_alpha", _alpha);

            while (mr.material.GetFloat("_alpha") > 0)
            {
                float newAlpha = mr.material.GetFloat("_alpha") - (_alpha / timeActive) * Time.deltaTime;
                mr.material.SetFloat("_alpha", Mathf.Max(newAlpha, 0));

                yield return null;  // Wait for the end of the frame
            }

            Debug.Log(timeActive);
            Destroy(gObj, meshDestroyDelay);
        }

        isTrailActive = false;
    }

    public  void OnPortalEvent()
    {
        isTrailActive2 = true;
        StartCoroutine(ActivateTrail2());
    }

    public void OnDodge()
    {
        StartCoroutine(ActivateTrail(activeTime));
    }
    public void DustLeft()
    {
        if (PlayerMovement.IsGrounded && PlayerMovement.effectsMoveCheck)
        {
        
           
                DustSystemLeft.Emit(6);
                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Elana Footstep Placeholder",  DustSystemLeft.gameObject);
            
          
        }
      
    }
    
    public void DustRight()
    {
        if (PlayerMovement.IsGrounded && PlayerMovement.effectsMoveCheck)
        {

        DustSystemRight.Emit(6);
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Elana Footstep Placeholder",  DustSystemRight.gameObject);
        }
    }

    public void OnLandedEvent()
    {
        OnLanded.Emit(10);
    }
}