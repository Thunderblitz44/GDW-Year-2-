using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyDetection : MonoBehaviour
{
    public EventHandler<Transform> onPlayerDetected;
    public EventHandler<Transform> onPlayerLost;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            onPlayerDetected?.Invoke(this, other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            onPlayerLost?.Invoke(this, other.transform);
        }
    }
}
