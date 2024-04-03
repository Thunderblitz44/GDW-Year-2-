using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class LIGHTNINGNUKE : Enemy
{
      public float buildupDelay = 3f;
    public float nukeDelay = 10f;
    public Volume localVolume;
    public Light pointLight; 
    private VisualEffect vfx;
    public ParticleSystem Nuke;
MeleeHitBox sword;
AttackTrigger trigger;


public MeleeHitBox[] RangedAttack;

private int RangedAttackDamage = 999;
public Vector2 RangedKnockback;

private void Start()
{
    
    RangedAttack = GetComponentsInChildren<MeleeHitBox>();
    foreach (var trigger in RangedAttack)
    {
        trigger.damage = RangedAttackDamage;
        trigger.knockback = RangedKnockback;
    }
}


void OnEnable()
    {
        
        vfx = GetComponent<VisualEffect>();
        if (vfx != null)
        {
            vfx.SendEvent("BUILDUP");
        }
     
        StartCoroutine(TriggerNukeAfterDelay());
    }

    IEnumerator TriggerNukeAfterDelay()
    {
        yield return new WaitForSeconds(buildupDelay);

        vfx.SendEvent("NUKE");
        Invoke("NukeVibration", 1f);
        Nuke.Play();
   
    
        yield return new WaitForSeconds(nukeDelay);

        Destroy(gameObject);
    }

    void NukeVibration()
    {
        StaticUtilities.ShakePlayerCamera(25, 2, 18);

        if (localVolume != null && pointLight != null)
        {
            StartCoroutine(LerpVolumeAndLightIntensity());
        }
    }

    IEnumerator LerpVolumeAndLightIntensity()
    {
        float elapsedTime = 0f;
        float duration = 1f;
        float startWeight = localVolume.weight;
        float targetWeight = 1f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            localVolume.weight = Mathf.Lerp(startWeight, targetWeight, t);

           
            pointLight.intensity = localVolume.weight * 5f; 

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        localVolume.weight = targetWeight;

        elapsedTime = 0f;
        float startWeightDown = targetWeight;
        float targetWeightDown = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            localVolume.weight = Mathf.Lerp(startWeightDown, targetWeightDown, t);

            
            pointLight.intensity = localVolume.weight * 5f; 

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        localVolume.weight = targetWeightDown;

    
        pointLight.intensity = 0f;
    }
    private void OnParticleCollision(GameObject other)
    {
        StaticUtilities.TryToDamage(other, RangedAttackDamage);
    }
}
