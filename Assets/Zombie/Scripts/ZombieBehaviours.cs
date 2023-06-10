using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Zombie))]
public class ZombieBehaviours : MonoBehaviour
{
    public enum Behaviour
    {
        passive,
        following,
        aggressive_chase,
        dying
    }

    Behaviour currentBehaviour;

    public EventHandler<Behaviour> onBehaviourChanged;

    public void ChangeBehaviour(Behaviour newBehaviour)
    {
        if (newBehaviour == currentBehaviour) return;
        onBehaviourChanged?.Invoke(this, newBehaviour);
        currentBehaviour = newBehaviour;
    }
}
