using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] GameObject hpBarPrefab;
    HPBar hpbar;
    float health;

    public Action onHealthZeroed;

    private void Awake()
    {
        hpbar = Instantiate(hpBarPrefab, GameSettings.instance.GetCanvas()).GetComponent<HPBar>();
        hpbar.maxHP = maxHealth;
        hpbar.SetHPValue01(1);
    }

    public void DeductHealth(float value)
    {
        health = Mathf.Clamp(health - value, 0, maxHealth);
        hpbar.ChangeHPByAmount(-value);
        if (health == 0) onHealthZeroed?.Invoke();
    }

    public float GetHealth() => health;
}
