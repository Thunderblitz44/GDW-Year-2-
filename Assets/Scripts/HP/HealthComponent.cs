using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] float maxHealth = 10f;
    [SerializeField] float height = 1.5f;
    [SerializeField] GameObject hpBarPrefab;
    HPBar hpbar;
    EntityHPBar entityHPBar;
    float health;

    public Action onHealthZeroed;

    private void Start()
    {
        health = maxHealth;

        if (hpBarPrefab.GetComponent<EntityHPBar>())
        {
            entityHPBar = Instantiate(hpBarPrefab, GameManager.Instance.worldCanvas).GetComponent<EntityHPBar>();
            entityHPBar.transform.position = transform.position + Vector3.up * height;
            entityHPBar.maxHP = maxHealth;
            entityHPBar.SetHPValue(health);
        }
        else if (hpBarPrefab.GetComponent<HPBar>())
        {
            hpbar = Instantiate(hpBarPrefab, GameManager.Instance.canvas).GetComponent<HPBar>();
            hpbar.maxHP = maxHealth;
            hpbar.SetHPValue(health);
        }
    }

    private void Update()
    {
        if (!entityHPBar) return;

        entityHPBar.transform.position = transform.position + Vector3.up * height;
    }

    public void DeductHealth(float value)
    {
        if (hpbar) hpbar.ChangeHPByAmount(-value);
        else if (entityHPBar) entityHPBar.ChangeHPByAmount(-value);
    }

    public float GetHealth() => hpbar == null ? entityHPBar == null ? 0 : entityHPBar.GetHP() : hpbar.GetHP();
}
