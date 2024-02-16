using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GolemBossScript : Enemy, IBossCommands
{
    [Header("Attack Pattern")]
    [SerializeField] float timeBetweenAttacks = 2;
    [SerializeField] float attackPrepareTime = 2;
    [SerializeField] float maxAttackReps = 2;
    float attackReps;
    int lastAttackIndex;
    int phase = 0;
    float atkTimer;
    bool isAttacking;
    readonly List<Func<IEnumerator>> attackFuncs = new();
    readonly List<DamageableEntity> crystals = new();

    [Header("Stomp Attack")]
    [SerializeField] int stompDamage = 10;
    [SerializeField] Vector2 stompKnockback = Vector2.one * 10f;
    [SerializeField] float stompCooldown = 5f;
    AttackTrigger[] stompTriggers;
    MeleeHitBox[] stompDamagers;
    readonly float[] stompCooldowns = new float[4] { 0, 0, 0, 0 };
    bool stomping;

    [Header("Lasers")]
    [SerializeField] int laserDamage;
    [SerializeField] float halfDistance = 10f;
    [SerializeField] float sweepSpeed = 1f;
    [SerializeField] float sweepSpeedIncrease = 0.5f;
    [SerializeField] float firstSweepDelay = 1f;
    [SerializeField] AnimationCurve laserCurve = AnimationCurve.Linear(0,0,1,1);
    [SerializeField] LineRenderer laserRenderer;
    [SerializeField] GameObject laserEmitter;
    [SerializeField] LayerMask laserObstacle;
    int sweeps = 2;

    [Header("Portal Projectiles")]
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] float shotDelay = 0.25f;
    [SerializeField] float shootingStartDelay = 1.5f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject portalPrefab;
    [SerializeField] BoxCollider spawnBounds;
    readonly List<ShootingPortal> pooledPortals = new();
    float shotDelayTimer;
    int portals = 3;

    [Header("Spawn Minions")]
    [SerializeField] int minionCount;
    [SerializeField] float minionPrepareTime = 10f;
    [SerializeField] float minionAttackCooldown = 10f;
    bool usingMinionAttack = false;

    [Header("Walking on walls")]
    [SerializeField] BoxCollider[] gotoWallsBase;
    [SerializeField] Transform[] gotoWalls;
    bool goingUp;
    
     bool MoveAcrossNavMeshesStarted;
    
 
   
    
    
    [Header("Phase 2+")]
    [SerializeField] float stunTime = 5f;


    // battle info
    bool battleStarted = false;
    float tempSpeed;

    protected override void Awake()
    {
        base.Awake();
        (hp as BossHealthComponent).nextPhase += NextPhase;
        crystals.AddRange(GetComponentsInChildren<DamageableEntity>().ToList());
        crystals.RemoveAt(0);

        stompTriggers = GetComponentsInChildren<AttackTrigger>();
        foreach (var trigger in stompTriggers)
        {
            trigger.onTriggerEnterNotify += StompAttack;
        }

        stompDamagers = GetComponentsInChildren<MeleeHitBox>(true);
        foreach (var trigger in stompDamagers)
        {
            trigger.damage = stompDamage;
            trigger.knockback = stompKnockback;
        }
    }

    protected override void Update()
    {
        if (!battleStarted) return;
        base.Update();
      
        if(agent.isOnOffMeshLink )
        {
            animator.SetBool("Jumping", true);
            StartCoroutine(MoveAcrossNavMeshLink(agent, 5f, 0.5f));
            MoveAcrossNavMeshesStarted=true;
        }
      
        // timers
        // attack checkers/counters
        // attacks
        if (attackFuncs.Count > 0 && !isAttacking && (atkTimer += Time.deltaTime) > timeBetweenAttacks)
        {
            isAttacking = true;
            atkTimer = 0;
            // pick an attack
            // start coroutine of attack
            newAtkInd:
            int attackIndex = UnityEngine.Random.Range(0, attackFuncs.Count);
            if (attackFuncs.Count > 1 && attackIndex == lastAttackIndex && ++attackReps >= maxAttackReps)
            {
                goto newAtkInd;
            }
            StartCoroutine(attackFuncs[attackIndex]());
            lastAttackIndex = attackIndex;
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

        for (int i = 0; i < stompCooldowns.Length; i++)
        {
            stompCooldowns[i] += Time.deltaTime;
        }
       

    }
    IEnumerator MoveAcrossNavMeshLink(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
       

        Vector3 startPos = agent.transform.position;
        Quaternion startRot = agent.transform.rotation;

        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        Quaternion endRot = Quaternion.LookRotation(data.endPos - data.startPos);

        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            agent.transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);
            normalizedTime += Time.deltaTime / duration;
            yield return null;
            agent.CompleteOffMeshLink();
           
        }
        animator.SetBool("Jumping", false);
    }
   // private void OnTriggerEnter(Collider other)
    //{
     //  for (int i = 0; i < gotoWallsBase.Length; i++)
    //  {
      //    if (gotoWallsBase[i].gameObject.activeSelf)
       //   {
        //      if (goingUp) target = gotoWalls[i];
          //   else
            //  {
              //    target = LevelManager.PlayerTransform;
                //   agent.speed = 3.5f;
               //}
               //gotoWallsBase[i].enabled = false;
          //}
        //}
    //}

    public void Introduce()
    {
        // entrance animation
        // pool portals
        projectile.owner = this;
        projectile.CheckPrefab();
        for (int i = 0; i < portals; i++)
        {
            ShootingPortal portal = Instantiate(portalPrefab).GetComponent<ShootingPortal>();
            portal.Setup(projectile, shotDelay, shootingStartDelay);
            pooledPortals.Add(portal);
            pooledPortals[i].gameObject.SetActive(false);
        }

        (hp as BossHealthComponent).Show();
        Invoke(nameof(StartBattle), 1f);
        NextPhase();
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
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Boss Laser", gameObject);
        yield return new WaitForSeconds(attackPrepareTime);

        Vector3 targetDir, start, end, sweepDir = Vector3.zero;

        for (int s = 0; s < sweeps; s++)
        {
            targetDir = LevelManager.PlayerTransform.position - laserEmitter.transform.position;
            if (s == 0) // left - right
            {
                sweepDir = Vector3.Cross(targetDir.normalized, Vector3.up);
            }
            else if (s == 1) // front - back
            {
                sweepDir = StaticUtilities.FlatDirection(LevelManager.PlayerTransform.position, laserEmitter.transform.position);
            }
            else  
            {
                Vector3 lr = Vector3.Cross(targetDir.normalized, Vector3.up);
                Vector3 fb = StaticUtilities.FlatDirection(LevelManager.PlayerTransform.position, laserEmitter.transform.position);
                if (s == 2) sweepDir = (fb + lr).normalized; // front right - back left
                else if (s == 3) sweepDir = (fb - lr).normalized; // front left - back right
            }
            start = LevelManager.PlayerTransform.position - sweepDir * halfDistance + targetDir * 2f;
            end = LevelManager.PlayerTransform.position + sweepDir * halfDistance + targetDir * 2f;

            laserRenderer.SetPosition(0, laserEmitter.transform.position);
            laserRenderer.SetPosition(1, start);

            if (s == 0) yield return new WaitForSeconds(firstSweepDelay);

            RaycastHit hit;
            for (float t = 0; t < 1; t += Time.deltaTime * (sweepSpeed + sweepSpeedIncrease * s)) 
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
        yield return new WaitForSeconds(attackPrepareTime);

        // spawn portals
        List<Vector3> portalPositions = new(pooledPortals.Count);
        float minDist = Mathf.Pow(2f, 2f);
        int itt;
        for (int i = 0; i < portals; i++)
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


    IEnumerator MinionsRoutine()
    {
        if (usingMinionAttack) goto end;
        usingMinionAttack = true;

        // go up wall
        agent.speed = 6;
        int wall = UnityEngine.Random.Range(0, gotoWallsBase.Length);
        target = gotoWallsBase[wall].transform;
        gotoWallsBase[wall].enabled = true;
        goingUp = true;


        yield return new WaitForSeconds(minionPrepareTime);
        isAttacking = false;
        LevelManager lmInstance = LevelManager.Instance;
        Bounds bossBounds = LevelManager.Instance.CurrentEncounter.EncounterBounds;

        for (int i = 0; i < minionCount; i++)
        {
            // pick random spot in the volume
            Vector3 spawnPoint = LevelManager.GetRandomEnemySpawnPoint(bossBounds);
            Vector3 playerPos = Vector3.right * LevelManager.PlayerTransform.position.x + Vector3.forward * LevelManager.PlayerTransform.position.z + Vector3.up * spawnPoint.y;
            Quaternion spawnRotation = LevelManager.PlayerTransform ? Quaternion.LookRotation(spawnPoint - playerPos, Vector3.up) : Quaternion.identity;
            LevelManager.spawnedEnemies.Add(Instantiate(lmInstance.LevelEnemyList[UnityEngine.Random.Range(0, lmInstance.LevelEnemyList.Count)], spawnPoint, spawnRotation).GetComponent<DamageableEntity>());
            yield return new WaitForSeconds(0.5f + i*0.5f);
        }

        // encounter loop
        while (LevelManager.spawnedEnemies.Count > 0)
        {
            for (int i = 0; i < LevelManager.spawnedEnemies.Count; i++)
            {
                if (LevelManager.spawnedEnemies[i] == null) LevelManager.spawnedEnemies.RemoveAt(i);
            }
            yield return new WaitForSeconds(0.1f);
        }

        // return from wall
        target = gotoWallsBase[wall].transform;
        gotoWallsBase[wall].enabled = false;
        goingUp = false;
        yield return new WaitForSeconds(minionAttackCooldown);
        usingMinionAttack = false;

    end:

        isAttacking = false;
        yield return null;
    }

    void NextPhase()
    {
        switch (++phase)
        {
            case 1:
                attackFuncs.Add(LasersRoutine);
                break;
            case 2:
                attackFuncs.Add(ProjectilesRoutine);
                animator.SetBool("ShieldsUp", true);
                foreach (var crystal in crystals) crystal.isInvincible = true;
                break;
            case 3:
                animator.SetBool("ShieldsUp", true);
                attackFuncs.Add(MinionsRoutine);
                sweeps++;
                timeBetweenAttacks = 
                attackPrepareTime = 
                shootingStartDelay = 1f;
                break;
            case 4:
                animator.SetBool("ShieldsUp", true);
                sweeps++;
                portals+=2;
                timeBetweenAttacks = 
                attackPrepareTime =
                shootingStartDelay = 0.5f;
                break;
        }
    }

    public override void ApplyDamage(int damage)
    {
    }

    public override void ApplyDamageOverTime(int damage, float duration)
    {
    }

    void StompAttack(GameObject sender)
    {
        for (int i = 0; i < stompTriggers.Length; i++)
        {
            if (stompTriggers[i].gameObject == sender && stompCooldowns[i] > stompCooldown)
            {
                stompCooldowns[i] = 0;
                animator.SetTrigger(sender.tag);
                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Boss Step", gameObject);
            }
        }
    }

    protected override void OnHealthZeroed()
    {
        StopAllCoroutines();

        (hp as BossHealthComponent).Hide();
        
        foreach (var portal in pooledPortals)
        {
            Destroy(portal.gameObject);
        }

        Destroy(gameObject, 1f);
    }

    public void Stun()
    {
        foreach (var crystal in crystals) crystal.isInvincible = false;
        animator.SetBool("ShieldsUp", false);
        Invoke(nameof(UnStun), stunTime);
    }

    void UnStun()
    {
        foreach (var crystal in crystals) crystal.isInvincible = true;
        animator.SetBool("ShieldsUp", true);
    }
}
