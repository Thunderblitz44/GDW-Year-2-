using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Encounter2 : EncounterVolume
{
    [SerializeField] int totalEnemies = 15;
    [SerializeField] int maxSpawnedAtOnce = 5;

    protected override IEnumerator EncounterRoutine()
    {
        yield return null;
        EndEncounter();
    }
}
