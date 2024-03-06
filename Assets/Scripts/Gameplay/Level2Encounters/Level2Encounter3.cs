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
        foreach (NavMeshSurface surface in previousNavmeshes)
        {
            surface.enabled = false;
        }

        for (int i = 0, c = LevelManager.spawnedEnemies.Count; i < c; i++)
        {
            Destroy(LevelManager.spawnedEnemies[0].gameObject);
            LevelManager.spawnedEnemies.RemoveAt(0);
        }

        do
        {
            // spawn enemies until cap
            if (LevelManager.spawnedEnemies.Count < maxSpawnedAtOnce && totalSpawned < totalEnemies)
            {
                yield return new WaitForSeconds(0.9f);
                SpawnEnemy();
            }

            CheckRemaining();
            yield return new WaitForSeconds(0.1f);
        } while (LevelManager.spawnedEnemies.Count > 0);

        EndEncounter();
    }
}
