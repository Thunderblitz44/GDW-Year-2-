using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject[] prefabsToInstantiate;
    public Transform[] spawnLocations = new Transform[4]; 

    void Start()
    {
        Invoke("SpawnAfterDelay", 15f);
    }

    void SpawnAfterDelay()
    {
        for (int i = 0; i < spawnLocations.Length; i++)
        {
            int randomIndex = Random.Range(0, prefabsToInstantiate.Length);
            GameObject randomPrefab = prefabsToInstantiate[randomIndex];

            Instantiate(randomPrefab, spawnLocations[i].position, Quaternion.identity);
        }
    }
}
