using System.Collections.Generic;
using UnityEngine;

public class ShootingPortal : DamageableEntity
{
    //int shotCount;
    ProjectileData projectile;
    float shotCooldown;
    float shotTimer;
    float startDelay;
    float startTimer;
    readonly List<GameObject> pooledProjectiles = new(4);
    bool settingUp = true;
    bool explode = false;
    bool stunned = false;
    public int bossStunDamage = 10;

    GameObject expl;

    protected override void Awake()
    {
        base.Awake();
        expl = transform.GetChild(0).gameObject;
        expl.GetComponent<AttackTrigger>().onTriggerEnter += TriggerEnter;
        expl.SetActive(false);
        (hp as EntityHealthComponent).DestroyOnHPZero = false;
    }

    public void Setup(ProjectileData projectile, float shotCooldown, float startDelay)
    {
        this.projectile = projectile;
        this.shotCooldown = shotCooldown;
        this.startDelay = startDelay;

        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectile.prefab).GetComponent<MagicBullet>();
            mb.Initialize(projectile);
            pooledProjectiles.Add(mb.gameObject);
        }
        settingUp = false;
    }

    private void OnDisable()
    {
        startTimer = 0;
        //shotCount = 0;
        explode = false;
        hp.SetHealth(hp.MaxHealth);
    }

    private void OnEnable()
    {
        hp.gameObject.SetActive(true);
        explode = false;

        Invoke(nameof(Die), 10f);
    }

    private void Update()
    {
        if (settingUp) return; 

        startTimer += Time.deltaTime;
        if (!explode && startTimer > startDelay && (shotTimer += Time.deltaTime) > shotCooldown) 
        {
            shotTimer = 0;
            Vector3 force = (LevelManager.PlayerTransform.position - transform.position).normalized * projectile.speed;
            StaticUtilities.ShootProjectile(pooledProjectiles, transform.position, force);
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Boss Bullet", gameObject);
        }

        transform.LookAt(LevelManager.PlayerTransform);
    }

    void TriggerEnter(Collider other)
    {
        if (stunned) return;
        if (other.CompareTag("GolemCrystal"))
        {
            other.GetComponent<BossWeakSpot>().Stun(bossStunDamage);
        }
    }

    protected override void OnHealthZeroed()
    {
        explode = true;
        expl.SetActive(true);
        Invoke(nameof(Die), 0.4f);
    }

    void Die()
    {
        expl.SetActive(false);
        gameObject.SetActive(false);
    }
}
