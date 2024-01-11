using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyBehaviours : MonoBehaviour
{
    public enum Behaviour
    {
        passive,
        following,
        aggressive_chase,
        attacking,
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
