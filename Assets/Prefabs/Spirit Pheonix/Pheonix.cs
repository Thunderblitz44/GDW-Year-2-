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
    [SerializeField] VisualEffect WingEffects1;
    [SerializeField] VisualEffect WingEffects2;
    [SerializeField] VisualEffect WingEffects3;
    [SerializeField] VisualEffect WingEffects4;
    void Start()
    {

        PheonixAnimator = GetComponent<Animator>();
        particleSystem3.GetComponent<MagicBullet>().Initialize(projectileFireball);
    }


    public void CastAttack()
    {
        PheonixAnimator.SetBool("IsCasting", true);
        PheonixAnimator.SetTrigger("TriggerCast");
        WingEffects1.SendEvent("StartEvent");
        WingEffects2.SendEvent("StartEvent");
        WingEffects3.SendEvent("StartEvent");
        WingEffects4.SendEvent("StartEvent");
    }


    public void EndAttack()
    {
        PheonixAnimator.SetBool("IsCasting", false);
        WingEffects1.SendEvent("EndEvent");
        WingEffects2.SendEvent("EndEvent");
        WingEffects3.SendEvent("EndEvent");
        WingEffects4.SendEvent("EndEvent");
    }


    public void SparkChargeEnable()
    {
        SparkCharge.Play();
        Invoke("Fireball", 2f);
    }

    public void Fireball()
    {
        FireBallParticleSystem.Play();
    }

    private void Update()
    {
        RaycastHit hit;
        Ray camLook = Camera.main.ScreenPointToRay(StaticUtilities.GetCenterOfScreen());

        if (Physics.Raycast(camLook, out hit))
        {
            Target.transform.position = hit.point;
        }
    }
}
