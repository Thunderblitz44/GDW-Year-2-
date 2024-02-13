using System;
using System.Collections.Generic;
using UnityEngine;

public class GolemRanger : Enemy
{
    // attack
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] Transform shootOrigin;
    [SerializeField] new ParticleSystem particleSystem;
    [SerializeField] GameObject HeadTarget;
    private float xSpeed;
    private float zSpeed;
    public float shootForce = 5;
    
    protected override void Awake()
    {
        base.Awake();


        particleSystem.GetComponent<MagicBullet>().Initialize(projectile, this);

        if (!shootOrigin) 
        { 
            Debug.LogWarning("No shoot origin set for ranger golem!");
            shootOrigin = transform;
        }
    }

    protected override void Update()
    {
        base.Update();
        HeadTarget.transform.position = LevelManager.PlayerTransform.position;

        float smoothingFactor = 0.1f;

        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity.normalized);

        // Smooth the velocity components (remove the float keyword)
        xSpeed = Mathf.Lerp(xSpeed, localVelocity.x, smoothingFactor);
        zSpeed = Mathf.Lerp(zSpeed, localVelocity.z, smoothingFactor);
        // Set the velocity values in the animator
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);

    }

    protected override void OnAttackTriggerEnter(Collider other)
    {
        //attack = true;
        animator.SetBool("InAttackRange", true);
    }

    protected override void OnAttackTriggerExit(Collider other)
    {
        animator.SetBool("InAttackRange", false);
    }

    public void EnableAI()
    {
        agent.enabled = true;
        HeadTarget.SetActive(true);
        target = LevelManager.PlayerTransform;
    }

    public void DisableAI()
    {
        agent.enabled = false;
        HeadTarget.SetActive(false);
    }
}
