using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHUD : MonoBehaviour
{
    [SerializeField] List<AbilityPoint> points;
    public Action<int> onPointRecharged;

    private void Awake()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].id = i;
            points[i].onAbilityPointCharged += onPointRecharged;
        }
    }

    public void SpendPoint(int id, float rechargeTime)
    {
        points[id].SetSpent();
        StartCoroutine(points[id].Recharge(rechargeTime));
    }

}
