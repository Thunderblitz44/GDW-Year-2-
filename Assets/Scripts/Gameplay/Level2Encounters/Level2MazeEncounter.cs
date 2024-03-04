using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2MazeEncounter : EncounterVolume
{
    Level2Puzzle puzzleManager;
    [SerializeField] int gem;

    protected override void Awake()
    {
        base.Awake();
        puzzleManager = GetComponentInParent<Level2Puzzle>();
    }

    protected override IEnumerator EncounterRoutine()
    {
        yield return base.EncounterRoutine();

        if (gem == 0) puzzleManager.BlueGemActivation();
        else if (gem == 1) puzzleManager.YellowGemActivation();
        else if (gem == 2) puzzleManager.GreenGemActivation();
    }
}
