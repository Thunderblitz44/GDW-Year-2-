using System;
using System.Collections.Generic;
using UnityEngine;

public class GolemRanger : Enemy
{
    // attack
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] float shootCooldown = 1.5f;
    [SerializeField] float shootStartDelay = 0.5f;
    [SerializeField] Transform shootOrigin;
    readonly List<GameObject> pooledProjectiles = new List<GameObject>(5);
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] GameObject HeadTarget;
    float shootStartTimer;
    float shootCooldownTimer;
    bool attack;
    private float xSpeed;
    private float ySpeed;
    private float zSpeed;
    public float shootForce = 5;
    
    protected override void Awake()
    {
        base.Awake();



        projectile.owner = this;
        projectile.CheckPrefab();
        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectile.prefab).GetComponent<MagicBullet>();
            mb.Initialize(projectile);
            pooledProjectiles.Add(mb.gameObject);
        }

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

        // attack cooldown + delay
        shootCooldownTimer += Time.deltaTime;
        if (shootCooldownTimer >= shootCooldown && attack &&
            (shootStartTimer += Time.deltaTime) >= shootStartDelay)
        {
            shootCooldownTimer = 0f;
            //Attack();
        }
        
        float smoothingFactor = 0.1f;

        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity.normalized);

        // Smooth the velocity components (remove the float keyword)
        xSpeed = Mathf.Lerp(xSpeed, localVelocity.x, smoothingFactor);
        zSpeed = Mathf.Lerp(zSpeed, localVelocity.z, smoothingFactor);
        ySpeed = Mathf.Lerp(ySpeed, localVelocity.y, smoothingFactor);
        // Set the velocity values in the animator
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
        animator.SetFloat("YSpeed", ySpeed);

    }

    protected override void OnAttackTriggerEnter(Collider other)
    {
        attack = true;
        animator.SetBool("InAttackRange", true);
    }

    protected override void OnAttackTriggerExit(Collider other)
    {
        attack = false;
        animator.SetBool("InAttackRange", false);
        shootStartTimer = 0f;
    }

    public void EnableAI()
    {
        agent.enabled = true;
        HeadTarget.SetActive(true);
        target = LevelManager.PlayerTransform;
    }

   // public void Attack()
   // {
       // Vector3 force = new Vector3(0f, shootForce, 0f);
       // StaticUtilities.ShootProjectile(pooledProjectiles, shootOrigin.position, force);
        
      //  animator.SetTrigger("Attack");
    //}

    protected override void OnHealthZeroed()
    {
        foreach (var projectile in pooledProjectiles)
        {
            projectile.GetComponent<MagicBullet>().Projectile.OwnerDestroyed();
        }

        base.OnHealthZeroed();
    }
    public void DisableAI()
    {
        agent.enabled = false;
        HeadTarget.SetActive(false);
        //target = LevelManager.PlayerTransform;
    }

    /*private void OnParticleCollision(GameObject other)
    {
        StaticUtilities.TryToDamage(other, projectile.damage);
    }*/
}
