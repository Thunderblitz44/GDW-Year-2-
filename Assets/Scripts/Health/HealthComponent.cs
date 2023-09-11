using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] GameObject hpBarPrefab;
    HPBar hpbar;
    EntityHPBar entityHPBar;
    float health;

    public Action onHealthZeroed;

    private void Start()
    {
        if (hpBarPrefab.GetComponent<EntityHPBar>())
        {
            entityHPBar = Instantiate(hpBarPrefab, GameSettings.instance.GetWorldCanvas()).GetComponent<EntityHPBar>();
            entityHPBar.transform.position = transform.position + Vector3.up * 1.5f;
            entityHPBar.maxHP = maxHealth;
            entityHPBar.SetHPValue01(1);
        }
        else if (hpBarPrefab.GetComponent<HPBar>())
        {
            hpbar = Instantiate(hpBarPrefab, GameSettings.instance.GetCanvas()).GetComponent<HPBar>();
            hpbar.maxHP = maxHealth;
            hpbar.SetHPValue01(1);
        }
    }

    public void DeductHealth(float value)
    {
        health = Mathf.Clamp(health - value, 0, maxHealth);

        if (hpbar) hpbar.ChangeHPByAmount(-value);
        else entityHPBar.ChangeHPByAmount(-value);

        if (health == 0) onHealthZeroed?.Invoke();
    }

    public float GetHealth() => health;
}
