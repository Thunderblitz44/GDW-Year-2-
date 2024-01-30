using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBossScript : Enemy, IBossCommands
{
    [Header("Attack Pattern")]
    [SerializeField] int minAtkReps = 1;
    [SerializeField] int maxAtkReps = 5;
    [SerializeField] float timeBetweenAttacks = 2;
    int atkRepetitions;
    int atkCounter;
    float atkTimer;
    bool isAttacking;

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
    [SerializeField] LineRenderer lrPrefab;
    [SerializeField] GameObject laserEmitter;

    // battle info
    bool battleStarted = false;
    float tempSpeed;

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
            StartCoroutine(ProjectilesRoutine());
            //StartCoroutine(LasersRoutine());
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
        // one big laser on top of boss
        // right-left or left right sweep 
        // front to back sweep
        
        yield return null;
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
            Vector3 spawnPoint = StaticUtilities.BuildVector(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
            
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
}
