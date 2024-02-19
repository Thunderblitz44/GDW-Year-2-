using System.Collections;

public class BossEncounter : EncounterVolume
{
    protected override IEnumerator EncounterRoutine()
    {
        LevelManager.Instance.Boss.GetComponent<IBossCommands>().Introduce();
        yield return null;
    }
}
