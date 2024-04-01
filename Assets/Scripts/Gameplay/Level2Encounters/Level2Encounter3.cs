using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class Level2Encounter3 : EncounterVolume
{
    [SerializeField] int totalEnemies = 10;
    [SerializeField] int maxSpawnedAtOnce = 3;
    [SerializeField] NavMeshSurface[] previousNavmeshes;

    protected override IEnumerator EncounterRoutine()
    {
        // Disable previous navmeshes
        foreach (NavMeshSurface surface in previousNavmeshes)
        {
            surface.enabled = false;
        }

        // Destroy objects named "Mantis Prefab" and "Salamander Rig"
        DestroyObjectsByName("Mantis Prefab");
        DestroyObjectsByName("Salamander Rig");

        // Clear spawned enemies list
        LevelManager.spawnedEnemies.Clear();

        int totalSpawned = 0;

        do
        {
            // Spawn enemies until cap
            if (LevelManager.spawnedEnemies.Count < maxSpawnedAtOnce && totalSpawned < totalEnemies)
            {
                yield return new WaitForSeconds(0.9f);
                SpawnEnemy();
                totalSpawned++;
            }

            CheckRemaining();
            yield return new WaitForSeconds(0.1f);
        } while (LevelManager.spawnedEnemies.Count > 0);

        // Enable previous navmeshes
        foreach (NavMeshSurface surface in previousNavmeshes)
        {
            surface.enabled = true;
        }

        EndEncounter();
    }

    private void DestroyObjectsByName(string objectName)
    {
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag(objectName);
        foreach (var obj in objectsToDestroy)
        {
            Destroy(obj);
        }
    }
}
