using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrailScript : MonoBehaviour
{
    public float activeTime = 2f;
    public bool isTrailActive;
    public float meshRefreshRate = 0.1f;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Material mat;
    public float meshDestroyDelay = 1f;
    public GameObject trailPrefab; // Reference to your trail prefab
    public bool isTrailActive2;
    public LayerMask groundLayer; // Specify the Ground layer in the Unity Editor
    public float meshRefreshRate2 = 0.1f;

    private Coroutine trailCoroutine;

    private void LateUpdate()
    {
        if (isTrailActive2 && trailCoroutine == null)
        {
            // Use Raycast logic to get position and rotation for Trail 2 with Y offset
            RaycastHit hit;
            Vector3 raycastOrigin = transform.position + new Vector3(0, 3f, 0); // Adding Y offset

            // Raycast only against objects on the "Ground" layer
            if (Physics.Raycast(raycastOrigin, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                // Start the coroutine and keep a reference to it
                trailCoroutine = StartCoroutine(ActivateTrail2(hit.point, Quaternion.LookRotation(hit.normal)));
            }
        }

        if (!isTrailActive2 && trailCoroutine != null)
        {
            StopCoroutine(trailCoroutine);
            trailCoroutine = null;

            // Disable the trail prefab
            trailPrefab.SetActive(false);
        }
    }

    private IEnumerator ActivateTrail2(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        // Enable the trail prefab
        trailPrefab.SetActive(true);

        while (isTrailActive2)
        {
            // Set the position directly from the hit point
            trailPrefab.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);

            // Match the rotation
            trailPrefab.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            // Perform any specific logic for Trail 2 here

            // Wait for the next iteration
            yield return new WaitForSeconds(meshRefreshRate2);
        }

        isTrailActive2 = false; // Disable the trail effect

        // Reset the coroutine reference when it's finished
        trailCoroutine = null;

        // Disable the trail prefab
        trailPrefab.SetActive(false);
    }
}