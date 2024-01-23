using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public NavMeshSurface navMesh { get; private set; }
    [SerializeField] List<EncounterVolume> encounterVolumes;
    [SerializeField] List<GameObject> enemies;
    readonly List<DamageableEntity> spawnedEnemies = new();

    [SerializeField] LayerMask groundLayer;

    EncounterVolume currentEncounter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("There were 2 LevelManager scripts!");
            Destroy(this);
            return;
        }
        Instance = this;

        for (int i = 0; i < encounterVolumes.Count; i++)
        {
            encounterVolumes[i].id = i;
            encounterVolumes[i].onEncounterStarted += StartEncounter;
        }

        navMesh = FindFirstObjectByType<NavMeshSurface>();
    }

    void StartEncounter(Bounds volumeBounds, int id)
    {
        currentEncounter = encounterVolumes[id];
        StartCoroutine(EncounterRoutine(volumeBounds));
    }

    IEnumerator EncounterRoutine(Bounds volumeBounds)
    {
        // start
        for (int i = 0; i < 5; i++)
        {
            // pick random spot in the volume
            Vector3 spawnPoint = GetRandomEnemySpawnPoint(volumeBounds);
            Vector3 playerPos = Vector3.right * StaticUtilities.playerTransform.position.x + Vector3.forward * StaticUtilities.playerTransform.position.z + Vector3.up * spawnPoint.y;
            Quaternion spawnRotation = StaticUtilities.playerTransform? Quaternion.LookRotation(spawnPoint - playerPos, Vector3.up) : Quaternion.identity;
            spawnedEnemies.Add(Instantiate(enemies[0], spawnPoint, spawnRotation).GetComponent<DamageableEntity>());

            yield return new WaitForSeconds(0.5f);
        }

        // encounter loop
        while (spawnedEnemies.Count > 0)
        {
            for(int i = 0;i < spawnedEnemies.Count;i++) 
            {
                if (spawnedEnemies[i] == null) spawnedEnemies.RemoveAt(i);
            }
            yield return new WaitForSeconds(0.1f);
        }

        // end
        currentEncounter.EndEncounter();
    }

    Vector3 GetRandomEnemySpawnPoint(Bounds volumeBounds)
    {
        int itterations = 0;
        Start:
        Vector3 spawnPoint = Vector3.right * Random.Range(volumeBounds.min.x, volumeBounds.max.x) + Vector3.up * volumeBounds.max.y + Vector3.forward * Random.Range(volumeBounds.min.z, volumeBounds.max.z);
        RaycastHit hit;
        if (Physics.Raycast(spawnPoint, Vector3.down, out hit, 100f, groundLayer, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Distance(hit.point, StaticUtilities.playerTransform.position) < 3) goto Start;

            foreach (var enemy in spawnedEnemies)
            {
                if (Vector3.Distance(enemy.transform.position, hit.point) < 2) goto Start;
            }

            return hit.point + Vector3.up;
        }
        if (++itterations < 20) goto Start;
        else
        {
            Debug.LogWarning("Couldn't find a spawn point! Returning a zero vector...");
            return Vector3.zero;
        }
    }
}
