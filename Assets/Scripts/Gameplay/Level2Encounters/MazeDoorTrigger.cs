using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDoorTrigger : MonoBehaviour
{
    Level2Puzzle puzzleManager;

    private void Awake()
    {
        puzzleManager = GetComponentInParent<Level2Puzzle>();
    }

    private void OnTriggerEnter(Collider other)
    {
        puzzleManager.OpenSesame();
    }
}
