using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class HeathPickup : MonoBehaviour
{
    [SerializeField] int healAmount = 50;
    [SerializeField] private VisualEffect HealthEffect;
    private void Update()
    {
      //  transform.rotation = Quaternion.LookRotation(StaticUtilities.FlatDirection(transform.position - LevelManager.PlayerTransform.position, Vector3.up));
    }

    private void OnTriggerEnter(Collider other)
    {
        StaticUtilities.TryToDamage(other.transform.parent.gameObject, -healAmount);
        HealthEffect.SendEvent("Pickup");
   Invoke("DelayedDisable", 3f);
   FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Placeholder Collectible Pickup", gameObject);
    }

  public void DelayedDisable()
    {
        gameObject.SetActive(false);
    }
}
