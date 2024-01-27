using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] internal float maxHealth = 10f;
    [SerializeField] internal GameObject hpBarPrefab;
    internal float health;
    HPBar hpbar;

    public Action onHealthZeroed;

    private void Awake()
    {
        health = maxHealth;

        hpbar = hpBarPrefab.GetComponent<HPBar>();
        hpbar.maxHP = maxHealth;
        hpbar.SetHPValue(health);
    }

    public virtual void DeductHealth(float value)
    {
        health = Mathf.Clamp(health - value, 0, maxHealth);

        if (hpbar) hpbar.ChangeHPByAmount(-value);

        if (health == 0)
        {
            onHealthZeroed?.Invoke();
        }
    }
}
