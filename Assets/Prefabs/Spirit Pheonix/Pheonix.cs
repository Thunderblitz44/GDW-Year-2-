using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Pheonix : MonoBehaviour
{
    private Animator PheonixAnimator;

    [SerializeField] private new ParticleSystem particleSystem3;
    [SerializeField] ProjectileData projectileFireball = ProjectileData.defaultProjectile;
    [SerializeField] VisualEffect VFX;
    [SerializeField] private ParticleSystem FireBallParticleSystem;
    [SerializeField] private ParticleSystem SparkCharge;
    private int totalParticlesEmitted = 0;
    private int totalParticlesToEmit = 10;
    public Transform Target;

   
    void Start()
    {
       
        PheonixAnimator = GetComponent<Animator>();
        particleSystem3.GetComponent<MagicBullet>().Initialize(projectileFireball);
    }

   
    public void CastAttack()
    {
     PheonixAnimator.SetBool("IsCasting", true);
     PheonixAnimator.SetTrigger("TriggerCast");
      
    }

    
    public void EndAttack()
    {
        PheonixAnimator.SetBool("IsCasting", false);
     
    }

  
    public void SparkChargeEnable()
    {
        SparkCharge.Play();
        Invoke("FireBall", 2f);
    }

    public void Fireball()
    {
        FireBallParticleSystem.Play();
    }

  
}
