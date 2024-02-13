using System;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public Action<Collider> onTriggerEnter;
    public Action<Collider> onTriggerExit;
    public Action<Collider> onTriggerStay;

    public Action<GameObject> onTriggerEnterNotify;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
        onTriggerEnterNotify?.Invoke(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        onTriggerStay?.Invoke(other);
    }
}
