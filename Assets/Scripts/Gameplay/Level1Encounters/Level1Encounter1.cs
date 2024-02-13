using System.Collections;
using UnityEngine;

public class Level1Encounter1 : EncounterVolume
{
    [SerializeField] int totalEnemies = 15;
    [SerializeField] int maxSpawnedAtOnce = 5;

    protected override IEnumerator EncounterRoutine()
    {
        // spawn 1 enemy
        SpawnEnemy(0);

        // wait
        while (LevelManager.spawnedEnemies.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
            CheckRemaining();
        }
        yield return new WaitForSeconds(2f);

        // spawn 3 enemies
        for (int i = 0; i < 3; i++)
        {
            SpawnEnemy(0);
            yield return new WaitForSeconds(0.5f);
        }

        // wait
        while (LevelManager.spawnedEnemies.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
            CheckRemaining();
        }

        // spawn the rest
        do
        {
            // spawn enemies until cap
            if (LevelManager.spawnedEnemies.Count < maxSpawnedAtOnce && totalSpawned < totalEnemies)
            {
                yield return new WaitForSeconds(0.4f);
                SpawnEnemy(0);
            }

            CheckRemaining();
            yield return new WaitForSeconds(0.1f);
        } while (LevelManager.spawnedEnemies.Count > 0);

        EndEncounter();
    }
}
