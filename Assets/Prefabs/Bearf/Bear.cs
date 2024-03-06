using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{
    // Reference to the prefab to instantiate
    public GameObject orbPrefab;
    public GameObject spearPrefab;

    // Reference to the transform to instantiate the orb at
    public Transform orbSpawnPoint;
    public Transform spearSpawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to perform the orb attack
    public void OrbAttack()
    {
        // Check if orbPrefab and orbSpawnPoint are set
        if (orbPrefab != null && orbSpawnPoint != null)
        {
            // Instantiate the prefab at the specified position and rotation
            Instantiate(orbPrefab, orbSpawnPoint.position, orbSpawnPoint.rotation);
        }
        else
        {
            Debug.LogError("orbPrefab or orbSpawnPoint is not set.");
        }
    }

    public void SpearAttack()
    {
        
        // Check if orbPrefab and orbSpawnPoint are set
        if (spearPrefab != null && spearSpawnPoint != null)
        {
            // Instantiate the prefab at the specified position and rotation
            Instantiate(spearPrefab, spearSpawnPoint.position, spearSpawnPoint.rotation);
        }
        else
        {
            Debug.LogError("orbPrefab or orbSpawnPoint is not set.");
        }
        
        
        
    }
}
