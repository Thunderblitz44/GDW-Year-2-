using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemRanger : Enemy
{
    // attack
    [SerializeField] float projectileDamage = 1f;
    [SerializeField] float projectileLifetime = 1f;
    [SerializeField] float shootCooldown = 1.5f;
    [SerializeField] float shootStartDelay = 0.5f;
    [SerializeField] float projectileSpeed = 20f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootOrigin;
    readonly List<GameObject> pooledProjectiles = new List<GameObject>(5);
    float shootStartTimer;
    float shootCooldownTimer;
    bool attack;

    internal override void Awake()
    {
        base.Awake();

        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectilePrefab).GetComponent<MagicBullet>();
            mb.damage = projectileDamage;
            mb.lifetime = projectileLifetime;
            mb.owner = this;
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

    internal override void OnTriggerEnter(Collider other)
    {
        attack = true;
    }

    internal override void OnTriggerExit(Collider other)
    {
        attack = false;
        shootStartTimer = 0f;
    }

    public void EnableAI()
    {
        agent.enabled = true;
        target = LevelManager.Instance.PlayerTransform;
    }

    void Attack()
    {
        foreach (var bullet in pooledProjectiles)
        {
            if (bullet.activeSelf) continue;

            bullet.SetActive(true);
            bullet.transform.position = shootOrigin.position;
            bullet.GetComponent<Rigidbody>().AddForce((shootOrigin.position - LevelManager.Instance.PlayerTransform.position).normalized * projectileSpeed, ForceMode.Impulse);
            break;
        }
        animator.SetTrigger("Attack");
        Debug.Log("shoot");
    }
}
