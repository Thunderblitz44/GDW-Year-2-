using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBossScript : Enemy, IBossCommands
{
    [Header("Attack Pattern")]
    [SerializeField] float timeBetweenAttacks = 2;
    int phase = 0;
    float atkTimer;
    bool isAttacking;
    readonly Func<IEnumerator>[] attackFuncs = new Func<IEnumerator>[2];

    [Header("Portal Projectiles")]
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] int shots = 5;
    [SerializeField] float shootingStartDelay = 1.5f;
    [SerializeField] float shotDelay = 0.25f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject portalPrefab;
    [SerializeField] BoxCollider spawnBounds;
    readonly List<ShootingPortal> pooledPortals = new(3);
    float shotDelayTimer;

    [Header("Lasers")]
    [SerializeField] float laserDamage;
    [SerializeField] float halfDistance = 10f;
    [SerializeField] float sweepSpeed = 1f;
    [SerializeField] float firstSweepDelay = 1f;
    [SerializeField] AnimationCurve laserCurve = AnimationCurve.Linear(0,0,1,1);
    [SerializeField] LineRenderer laserRenderer;
    [SerializeField] GameObject laserEmitter;
    [SerializeField] LayerMask laserObstacle;

    [Header("Spawn Minions")]
    [SerializeField] int minionCount;
    [SerializeField] int maxSpawned;

    // battle info
    bool battleStarted = false;
    float tempSpeed;

    internal override void Awake()
    {
        base.Awake();
        (hp as BossHealthComponent).nextPhase += NextPhase;
        attackFuncs[0] = ProjectilesRoutine;
        attackFuncs[1] = LasersRoutine;
    }

    internal override void Update()
    {
        if (!battleStarted) return;
        base.Update();

        // timers
        // attack checkers/counters
        // attacks
        if (!isAttacking && (atkTimer += Time.deltaTime) > timeBetweenAttacks)
        {
            Debug.Log("new attack");
            isAttacking = true;
            atkTimer = 0;
            // pick an attack
            // start coroutine of attack
            StartCoroutine(attackFuncs[UnityEngine.Random.Range(0,attackFuncs.Length)]());
        }

        if (Vector3.Distance(LevelManager.PlayerTransform.position, transform.position) <= agent.stoppingDistance && tempSpeed == 0)
        {
            tempSpeed = agent.speed;
            agent.speed = 0.1f;
        }
        else if (tempSpeed > 0)
        {
            agent.speed = tempSpeed;
            tempSpeed = 0;
        }
    }

    public void Introduce()
    {
        // entrance animation
        // pool portals
        projectile.owner = this;
        projectile.CheckPrefab();
        for (int i = 0; i < pooledPortals.Capacity; i++)
        {
            ShootingPortal portal = Instantiate(portalPrefab).GetComponent<ShootingPortal>();
            portal.Setup(projectile,shots, shotDelay, shootingStartDelay);
            pooledPortals.Add(portal);
            pooledPortals[i].gameObject.SetActive(false);
        }

        (hp as BossHealthComponent).Show();
        Invoke(nameof(StartBattle), 1f);
    }

    void StartBattle()
    {
        battleStarted = true;
        target = LevelManager.PlayerTransform;
    }

    IEnumerator LasersRoutine()
    {
        float temp = agent.speed;
        agent.speed = 0.1f;

        List<GameObject> damagedEntities = new();
        laserEmitter.SetActive(true);
        yield return new WaitForSeconds(2f);

        Vector3 targetDir, start, end, sweepDir = Vector3.zero;

        for (int s = 0; s < 2; s++)
        {
            targetDir = LevelManager.PlayerTransform.position - laserEmitter.transform.position;
            if (s == 0) // left - right
            {
                sweepDir = Vector3.Cross(targetDir.normalized, Vector3.up);
            }
            else if (s == 1) // front - back
            {
                sweepDir = StaticUtilities.FlatDirection(LevelManager.PlayerTransform.position, laserEmitter.transform.position).normalized;
            }
            start = LevelManager.PlayerTransform.position - sweepDir * halfDistance + targetDir * 2f;
            end = LevelManager.PlayerTransform.position + sweepDir * halfDistance + targetDir * 2f;

            laserRenderer.SetPosition(0, laserEmitter.transform.position);
            laserRenderer.SetPosition(1, start);

            if (s == 0) yield return new WaitForSeconds(firstSweepDelay);

            RaycastHit hit;
            for (float t = 0; t < 1; t += Time.deltaTime * sweepSpeed) 
            {
                Vector3 newPos = Vector3.Lerp(start, end, t);
                targetDir = newPos - laserEmitter.transform.position;
                if (Physics.Raycast(laserEmitter.transform.position, targetDir, out hit, 100, laserObstacle, QueryTriggerInteraction.Ignore))
                {
                    laserRenderer.SetPosition(1, hit.point);

                    // only damage things once
                    if (!damagedEntities.Contains(hit.transform.gameObject) &&
                        StaticUtilities.TryToDamage(hit.transform.gameObject, laserDamage))
                    {
                        // stop damaging that entity
                        damagedEntities.Add(hit.transform.gameObject);
                    }
                }
                else
                {
                    laserRenderer.SetPosition(1, newPos);
                }
                yield return null;
            }

            ResetLaser();
            damagedEntities.Clear();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        laserEmitter.SetActive(false);

        yield return null;
        isAttacking = false;
        agent.speed = temp;
    }

    void ResetLaser()
    {
        laserRenderer.SetPosition(0, laserEmitter.transform.position);
        laserRenderer.SetPosition(1, laserEmitter.transform.position);
    }

    IEnumerator ProjectilesRoutine()
    {
        float temp = agent.speed;
        agent.speed = 0.1f;
        yield return new WaitForSeconds(shootingStartDelay);

        // spawn portals
        List<Vector3> portalPositions = new(pooledPortals.Count);
        float minDist = Mathf.Pow(2f, 2f);
        int itt;
        for (int i = 0; i < pooledPortals.Count; i++)
        {
            itt = 0;
        getPos:

            Vector3 min = spawnBounds.bounds.min;
            Vector3 max = spawnBounds.bounds.max;
            Vector3 spawnPoint = StaticUtilities.BuildVector(UnityEngine.Random.Range(min.x, max.x), 
                UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
            
            // check if we like this pos
            foreach (var pos in portalPositions)
            {
                // dont be too close to another portal and must be within the arena bounds
                if (StaticUtilities.FastDistance(spawnPoint, pos) < minDist)
                {
                    if (++itt > 20)
                    {
                        Debug.Log("Can't make portal spawn pos! Skipping attack...");
                        goto end;
                    }
                    goto getPos;
                }
            }

            portalPositions.Add(spawnPoint);

            // spawn
            foreach (var portal in pooledPortals)
            {
                if (portal.gameObject.activeSelf) continue;

                portal.gameObject.SetActive(true);
                portal.transform.position = spawnPoint;
                break;
            }
            yield return new WaitForSeconds(0.3f);
        }
        
        // end
        end:

        // wait for all portals to deactivate
        for (int i = 0; i < pooledPortals.Count;)
        {
            if (!pooledPortals[i].gameObject.activeSelf) i++;
            yield return null;
        }

        yield return null;
        isAttacking = false;
        agent.speed = temp;
    }


    void SpawnMinions()
    {
        StartCoroutine(LevelManager.Instance.EncounterRoutine(LevelManager.Instance.CurrentEncounter.EncounterBounds));
    }

    void NextPhase()
    {
        switch (++phase)
        {
            case 1:
                Debug.Log("phase 2");
                // phase 2
                break;
            case 2:
                Debug.Log("phase 3");
                // phase 3
                break;
            case 3:
                Debug.Log("phase 4");
                // phase 4
                break;

        }
    }

    public override void ApplyDamage(float damage)
    {
    }

    public override void ApplyDamageOverTime(float dps, float duration)
    {
    }

    internal override void OnHealthZeroed()
    {
        StopAllCoroutines();
        
        foreach (var portal in pooledPortals)
        {
            Destroy(portal.gameObject);
        }

        Destroy(gameObject, 1f);
    }
}
