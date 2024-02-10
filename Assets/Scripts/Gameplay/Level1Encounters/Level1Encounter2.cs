using System.Collections;
using UnityEngine;

public class Level1Encounter2 : EncounterVolume
{
    [SerializeField] int totalEnemies = 20;
    [SerializeField] int maxSpawnedAtOnce = 5;
    [SerializeField] GameObject[] firstSpawnPoints = new GameObject[3];

    protected override IEnumerator EncounterRoutine()
    {
        // spawn 3 enemies
        for (int i = 0; i < 3; i++)
        {
            SpawnEnemy(firstSpawnPoints[i].transform.position,1);
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
                SpawnEnemy();
            }

            CheckRemaining();
            yield return new WaitForSeconds(0.1f);
        } while (LevelManager.spawnedEnemies.Count > 0);

        EndEncounter();
    }
}
