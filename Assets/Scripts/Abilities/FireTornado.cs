using System;
using System.Collections.Generic;
using UnityEngine;

public class FireTornado : MonoBehaviour
{
    [HideInInspector] public float burnTime = 0;
    [HideInInspector] public float damage = 0;

    List<GameObject> enemiesInTornado = new();
    List<float> damageTimers = new();

    private void Awake()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Fire Tornado Placeholder");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemiesInTornado.Add(other.gameObject);
            damageTimers.Add(0);
        }
    }

    private void Update()
    {
        for (int i = 0; i < enemiesInTornado.Count; i++)
        {
            if (!enemiesInTornado[i])
            {
                damageTimers.RemoveAt(i);
                enemiesInTornado.RemoveAt(i);
                continue;
            }

            if ((damageTimers[i] += Time.deltaTime) < StaticUtilities.damageOverTimeInterval) continue;
 
            damageTimers[i] = 0;
            StaticUtilities.TryToDamage(enemiesInTornado[i], damage * StaticUtilities.damageOverTimeInterval);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            StaticUtilities.TryToDamageOverTime(other.gameObject, damage, burnTime);
            damageTimers.RemoveAt(enemiesInTornado.IndexOf(other.gameObject));
            enemiesInTornado.Remove(other.gameObject);
        }
    }
}
