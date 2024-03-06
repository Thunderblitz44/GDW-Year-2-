using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEncounter2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0, c = LevelManager.spawnedEnemies.Count; i < c; i++)
        {
            Destroy(LevelManager.spawnedEnemies[0].gameObject);
            LevelManager.spawnedEnemies.RemoveAt(0);
        }
    }
}
