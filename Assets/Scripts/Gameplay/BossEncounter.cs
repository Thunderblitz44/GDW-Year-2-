using System.Collections;
using FMOD;
using UnityEngine;

public class BossEncounter : EncounterVolume
{
    [SerializeField] GameObject teleporter;
   
    protected override IEnumerator EncounterRoutine()
    {
        LevelManager.Instance.Boss.GetComponent<IBossCommands>().Introduce();
        LevelManager.Instance.BossMusic = true;
        yield return null;
    }

    public override void EndEncounter()
    {
        base.EndEncounter();
        LevelManager.Instance.BossMusic = false;
        if (teleporter) teleporter.SetActive(true);
    }
}
