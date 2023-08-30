using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] float maxHealth;
    float health;

    public Action onHealthZeroed;

    public void DeductHealth(float value)
    {
        health = Mathf.Clamp(health - value, 0, maxHealth);

        if (health == 0) onHealthZeroed?.Invoke();
    }

    public float GetHealth() => health;
}
