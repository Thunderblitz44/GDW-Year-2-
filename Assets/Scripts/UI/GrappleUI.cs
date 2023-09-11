using System;
using UnityEngine;

public class GrappleUI : MonoBehaviour
{
    [SerializeField] AbilityPoint point;
    public Action onGrappleRecharged;

    private void Awake()
    {
        point.onAbilityPointCharged += OnGrappleCharged;
    }

    public void SpendPoint()
    {
        point.SetSpent();
    }

    public void RechargePoint(float totalTime)
    {
        StartCoroutine(point.Recharge(totalTime));
    }

    void OnGrappleCharged()
    {
        onGrappleRecharged?.Invoke();
    }
}
