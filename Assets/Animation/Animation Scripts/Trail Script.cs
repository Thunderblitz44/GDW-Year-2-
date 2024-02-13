using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailScript : MonoBehaviour
{
    public Transform positionToSpawn;
    public float activeTime = 2f;
    public bool isTrailActive;
    public float meshRefreshRate = 0.1f;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Material mat;
    public float meshDestroyDelay = 1f;


    void FixedUpdate()
    {
        if (isTrailActive)
        {
            StartCoroutine(ActivateTrail(activeTime));
        }
    }

    IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
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
            yield return new WaitForSeconds(meshRefreshRate);


            Destroy(gObj, meshDestroyDelay);
        }

        isTrailActive = false;
    }
}