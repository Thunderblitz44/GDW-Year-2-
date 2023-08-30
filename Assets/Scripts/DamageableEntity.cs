using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity : MonoBehaviour, IDamageable
{
    HealthComponent hp;
    private void Awake()
    {
        hp = GetComponent<HealthComponent>();
        hp.onHealthZeroed += OnHealthZeroed;
    }

    internal virtual void OnHealthZeroed()
    {

    }

    public void ApplyDamage(float damage)
    {
        hp.DeductHealth(damage);
    }

    public void ApplyDamageOverTime(float dps, float duration)
    {
        StartCoroutine(DamageOverTimeRoutine(dps, duration));
    }

    IEnumerator DamageOverTimeRoutine(float dps, float duration)
    {
        for (float i = 0, n = 0; i < duration; i += Time.deltaTime, n += Time.deltaTime)
        {
            if (n > GameSettings.instance.damageOverTimeInterval)
            {
                n = 0;
                ApplyDamage(dps * GameSettings.instance.damageOverTimeInterval);
            }
            yield return null;
        }
    }
}
