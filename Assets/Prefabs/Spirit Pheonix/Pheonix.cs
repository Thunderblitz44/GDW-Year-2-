using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheonix : MonoBehaviour
{
    private Animator PheonixAnimator;

    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    // Start is called before the first frame update
    void Start()
    {
        PheonixAnimator = GetComponent<Animator>();
        particleSystem.GetComponent<MagicBullet>().Initialize(projectile);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
