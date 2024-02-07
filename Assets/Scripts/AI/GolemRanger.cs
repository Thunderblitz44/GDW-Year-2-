using System.Collections;
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
    float shootStartTimer;
    float shootCooldownTimer;
    bool attack;

    internal override void Awake()
    {
        base.Awake();

        projectile.owner = this;
        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectile.prefab).GetComponent<MagicBullet>();
            mb.Projectile = projectile;
            pooledProjectiles.Add(mb.gameObject);
        }

        if (!shootOrigin) 
        { 
            Debug.LogWarning("No shoot origin set for ranger golem!");
            shootOrigin = transform;
        }
    }

    internal override void Update()
    {
        base.Update();

        // attack cooldown + delay
        shootCooldownTimer += Time.deltaTime;
        if (shootCooldownTimer >= shootCooldown && attack &&
            (shootStartTimer += Time.deltaTime) >= shootStartDelay)
        {
            shootCooldownTimer = 0f;
            Attack();
        }
    }

    internal override void OnAttackTriggerEnter(Collider other)
    {
        attack = true;
    }

    internal override void OnAttackTriggerExit(Collider other)
    {
        attack = false;
        shootStartTimer = 0f;
    }

    public void EnableAI()
    {
        agent.enabled = true;
        target = LevelManager.PlayerTransform;
    }

    void Attack()
    {
        Vector3 force = (shootOrigin.position - LevelManager.PlayerTransform.position).normalized * projectile.speed;
        StaticUtilities.ShootProjectile(pooledProjectiles, shootOrigin.position, force);
        
        animator.SetTrigger("Attack");
    }
}
