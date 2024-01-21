using System;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    public Action<Collision> onCollisionEnter;
    public Action<Collision> onCollisionExit;
    public Action<Collision> onCollisionStay;

    private void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter?.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        onCollisionExit?.Invoke(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        onCollisionStay?.Invoke(collision);
    }
}
