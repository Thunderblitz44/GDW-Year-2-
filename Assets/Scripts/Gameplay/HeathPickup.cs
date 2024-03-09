using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeathPickup : MonoBehaviour
{
    [SerializeField] int healAmount = 50;

    private void OnTriggerEnter(Collider other)
    {
        StaticUtilities.TryToDamage(other.transform.parent.gameObject, -healAmount);
        gameObject.SetActive(false);
    }
}
