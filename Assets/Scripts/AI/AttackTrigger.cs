using System;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public Action<Collider> onTriggerEnter;
    public Action<Collider> onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }
}
