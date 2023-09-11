using System;
using System.Collections.Generic;
using UnityEngine;

public class DashUI : MonoBehaviour
{
    [SerializeField] Transform gridGroup;
    [SerializeField] GameObject dashPointPrefab;
    List<AbilityPoint> points = new();
    int chargedDashes;
    public Action onDashesRecharged;

    public void SetDashVisual(int maxDashes)
    {
        // add or remove points
        if (gridGroup.childCount > maxDashes)
        {
            int diff = gridGroup.childCount - maxDashes;
            for (int i = 0; i < diff; i++)
            {
                Destroy(gridGroup.GetChild(i).gameObject);
            }
        }
        else
        {
            int diff = maxDashes - gridGroup.childCount;
            for (int i = 0; i < diff; i++)
            {
                Instantiate(dashPointPrefab, gridGroup);
            }
        }

        // make a list of all points
        for (int i = 0; i < gridGroup.childCount; i++)
        {
            points.Add(gridGroup.GetChild(i).GetComponent<AbilityPoint>());
            points[i].onAbilityPointCharged += OnDashCharged;
        }
    }

    public void SpendDash(bool all = false)
    {
        for(int i = points.Count - 1; i >= 0; i--)
        {
            if (points[i].isSpent) continue;
            
            points[i].SetSpent();
            if (all == false) break;
        }
    }

    public void RechargeDashes(float totalTime)
    {
        foreach (var point in points)
        {
            StartCoroutine(point.Recharge(totalTime));
        }
    }

    void OnDashCharged()
    {
        if (++chargedDashes >= points.Count) onDashesRecharged?.Invoke();
    }
}
