using System.Collections;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Level2Encounter2 : EncounterVolume
{
 [SerializeField] NavMeshSurface previousNavmesh;
[SerializeField] NavMeshSurface[] navmeshes;
private Vector3 targetTransform = new Vector3(0, 0, 0);
protected override IEnumerator EncounterRoutine()
{
    previousNavmesh.enabled = false;

    for (int i = 0; i < 5; i++)
    {
        if (navmeshes[0].isActiveAndEnabled) SpawnEnemy(1);
        yield return new WaitForSeconds(1);
    }
}

private void OnTriggerExit(Collider other)
{

    if (other.CompareTag("Player"))
    {
        Doom();
    }
}
private void Doom()
{
    Debug.Log("YOUMAMA");
LevelManager.Instance.EndEncounterMusic();
}
}
