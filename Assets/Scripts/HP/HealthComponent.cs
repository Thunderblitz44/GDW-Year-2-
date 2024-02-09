using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 10;
    public int MaxHealth { get { return maxHealth; } }
    [SerializeField] protected GameObject hpBarPrefab;
    protected int health;
    public int Health { get { return health; } }
    HPBar hpbar;

    public Action onHealthZeroed;

    private void Awake()
    {
        health = maxHealth;

        hpbar = hpBarPrefab.GetComponent<HPBar>();
        hpbar.maxHP = maxHealth;
        hpbar.SetHPValue(health);
    }

    public virtual void DeductHealth(int value)
    {
        health = Mathf.Clamp(health - value, 0, maxHealth);

        if (hpbar) hpbar.ChangeHPByAmount(-value);

        if (health == 0)
        {
            onHealthZeroed?.Invoke();
        }
    }

    public void SetHealth(int value)
    {
        health = value;
        hpbar.SetHPValue(health);
    }
}
