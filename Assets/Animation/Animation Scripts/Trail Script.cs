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
    private ParticleSystem TrailRibbon;
    public float _alpha = 0.38f;

    void Start()
    {
        TrailRibbon = GetComponent<ParticleSystem>();



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
        TrailRibbon.Play();
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

            // Initialize alpha value
            float alphaValue = 1.0f;

            // Define lerping duration
            float lerpDuration = 1.0f; // You can adjust this duration as needed

            // Coroutine for lerping alpha value
            IEnumerator LerpAlpha()
            {
                float elapsed = 0.0f;
                while (elapsed < lerpDuration)
                {
                    alphaValue = Mathf.Lerp(1.0f, 0.0f, elapsed / lerpDuration);
                    mat.SetFloat("_AlphaThres", alphaValue);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // Ensure the final alpha value is set
                alphaValue = 0.0f;
                mat.SetFloat("_AlphaThres", alphaValue);
            }

            // Start lerping alpha value
            StartCoroutine(LerpAlpha());

            //Debug.Log(timeActive);
            Destroy(gObj, meshDestroyDelay);

            yield return new WaitForSeconds(0.1f);
        }
        isTrailActive = false;
        TrailRibbon.Stop();
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
        if (PlayerMovement.IsGrounded && PlayerMovement.IsMoving)
        {
        
           
                DustSystemLeft.Emit(6);
                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Elana Footstep Placeholder",  DustSystemLeft.gameObject);
            
          
        }
      
    }
    
    public void DustRight()
    {
        if (PlayerMovement.IsGrounded && PlayerMovement.IsMoving)
        {

        DustSystemRight.Emit(6);
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Elana Footstep Placeholder",  DustSystemRight.gameObject);
        }
    }

    public void OnLandedEvent()
    {
        OnLanded.Emit(10);
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Warrior Footsteps",  DustSystemRight.gameObject);
    }
}