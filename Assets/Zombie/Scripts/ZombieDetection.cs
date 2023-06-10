using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Zombie))]
public class ZombieDetection : MonoBehaviour
{
    public EventHandler<Transform> onPlayerDetected;
    public Action onPlayerLost;

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
            onPlayerLost?.Invoke();
        }
    }
}
