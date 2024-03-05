using System.Collections;
using UnityEngine;

public class BossEncounter : EncounterVolume
{
    [SerializeField] GameObject teleporter;

    protected override IEnumerator EncounterRoutine()
    {
        LevelManager.Instance.Boss.GetComponent<IBossCommands>().Introduce();
        yield return null;
    }

    public override void EndEncounter()
    {
        base.EndEncounter();

        teleporter.SetActive(true);
    }
}
