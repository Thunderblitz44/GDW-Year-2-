using System;
using System.Collections.Generic;
using UnityEngine;

public class FireTornado : MonoBehaviour
{
    public float BurnTime { private get;  set; } = 0;
    public int Damage { private get; set;} = 0;

    readonly List<GameObject> enemiesInTornado = new();
    readonly List<float> damageTimers = new();

    private void Awake()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Fire Tornado Placeholder", gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player") ||
            other.gameObject.layer != LayerMask.NameToLayer("Friendly"))
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
            StaticUtilities.TryToDamage(enemiesInTornado[i], Damage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player") || 
            other.gameObject.layer != LayerMask.NameToLayer("Friendly"))
        {
            StaticUtilities.TryToDamageOverTime(other.gameObject, Damage, BurnTime);
            damageTimers.RemoveAt(enemiesInTornado.IndexOf(other.gameObject));
            enemiesInTornado.Remove(other.gameObject);
        }
    }
}
