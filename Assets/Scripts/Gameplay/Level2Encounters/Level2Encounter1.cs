using System.Collections;
using UnityEngine;

public class Level2Encounter1 : EncounterVolume
{
    [SerializeField] int totalEnemies = 10;
    [SerializeField] int maxSpawnedAtOnce = 3;

    protected override IEnumerator EncounterRoutine()
    {
        // spawn 1 salamander
        SpawnEnemy(0);

        // wait
        while (LevelManager.spawnedEnemies.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
            CheckRemaining();
        }
        yield return new WaitForSeconds(2f);

        // spawn 1 mantis
        SpawnEnemy(1);

        // wait
        while (LevelManager.spawnedEnemies.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
            CheckRemaining();
        }
        yield return new WaitForSeconds(2f);

        // spawn the rest
        do
        {
            // spawn enemies until cap
            if (LevelManager.spawnedEnemies.Count < maxSpawnedAtOnce && totalSpawned < totalEnemies)
            {
                yield return new WaitForSeconds(0.4f);
                SpawnEnemy();
            }

            CheckRemaining();
            yield return new WaitForSeconds(0.1f);
        } while (LevelManager.spawnedEnemies.Count > 0);

        EndEncounter();
    }
}
