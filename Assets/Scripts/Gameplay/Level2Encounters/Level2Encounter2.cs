using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class Level2Encounter2 : EncounterVolume
{
    [SerializeField] NavMeshSurface previousNavmesh;
    [SerializeField] NavMeshSurface[] navmeshes;
    protected override IEnumerator EncounterRoutine()
    {
        previousNavmesh.enabled = false;
        // spawn 5 mantis
        for (int i = 0; i < 5; i++)
        {
            if (navmeshes[0].isActiveAndEnabled) SpawnEnemy(1);
            yield return new WaitForSeconds(1);
        }
    }
}
