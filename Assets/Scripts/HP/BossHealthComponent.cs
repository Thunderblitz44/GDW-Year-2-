using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthComponent : HealthComponent
{
    [SerializeField] Animator animator;

    public void Show()
    {
        hpBarPrefab.SetActive(true);
        // play anim
    }

    public void Hide()
    {
        hpBarPrefab.SetActive(false);
        // play anim
    }
}
