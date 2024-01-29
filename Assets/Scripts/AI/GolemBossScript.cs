using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GolemBossScript : Enemy, IBossCommands
{
    [Header("Attack Pattern")]
    [SerializeField] int minAtkReps = 1;
    [SerializeField] int maxAtkReps = 5;
    int atkRepetitions;
    int atkCounter;

    [Header("Portal Projectiles")]
    [SerializeField] int shots = 5;
    [SerializeField] int portals = 3;
    [SerializeField] float shotDelay = 0.25f;
    [SerializeField] float shotSpeed = 30f;
    [SerializeField] float shotDamage = 1;
    [SerializeField] float shotLife = 1;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject portalPrefab;
    readonly List<GameObject> pooledPortals = new(5);
    readonly List<GameObject> pooledProjectiles = new(10);

    [Header("Lasers")]
    [SerializeField] float laserDamage;
    [SerializeField] LineRenderer lrPrefab;

    // battle info
    bool battleStarted = false;

    internal override void Awake()
    {
        base.Awake();
        hp = GetComponentInChildren<BossHealthComponent>();
        hp.onHealthZeroed += () => 
        {
            (hp as BossHealthComponent).Hide();
            Destroy(gameObject); 
        };
    }

    internal override void Update()
    {
        if (battleStarted) base.Update();
    }

    public void Introduce()
    {
        // entrance animation

        // pool portals
        for (int i = 0; i < pooledPortals.Capacity; i++)
        {
            pooledPortals.Add(Instantiate(portalPrefab));
        }

        // pool projectiles
        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectilePrefab).GetComponent<MagicBullet>();
            mb.damage = shotDamage;
            mb.lifetime = shotLife;
            mb.owner = this;
            pooledProjectiles.Add(mb.gameObject);
        }

        (hp as BossHealthComponent).Show();
        Invoke(nameof(StartBattle), 1f);
    }

    void StartBattle()
    {
        battleStarted = true;
        target = LevelManager.Instance.PlayerTransform;
    }
}
