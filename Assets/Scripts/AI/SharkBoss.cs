using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class SharkBoss : Enemy, IBossCommands
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

    bool battleStarted = false;

    bool interrupt = false;

    [Header("Intro")]
    public bool skipIntro = false;
    [SerializeField] SplineContainer introPath;
    [SerializeField] AnimationCurve speedCurve;

    [Header("Passive Path")]
    [SerializeField] SplineContainer passivePath;
    float speed = 0.1f;
    float passivePathTime;
    float passivePathSpeed = 0.1f;
    float lerpToLinkSpeed = 4f;
    float pathRatio;
    float distToPath;

    int targetLink;

    bool isOnPath;
    bool findClosestLink;
    bool calculatePosAndRot;
    bool followPassivePath;

    Vector3 startedPos;
    Vector3 closestLinkPos;

    Quaternion lookingAtQuat;
    Quaternion startedQuat;

    [Header("Dive Attack")]
    [SerializeField] GameObject aoeIndicatorPrefab;
    GameObject aoeIndicator;
    [SerializeField] AnimationCurve diveHCurve;
    [SerializeField] AnimationCurve diveVCurve;

    [Header("Ram Attack")]
    [SerializeField] AnimationCurve ramCurve;
    [SerializeField] AnimationCurve expCurve;
    [SerializeField] AnimationCurve logCurve;

    [Header("Shoot Attack")]
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] int shots = 3;
    [SerializeField] float chargeTime = 1;
    [SerializeField] float cooldown = 3;
    [SerializeField] Transform shootOrigin;
    [SerializeField] LayerMask hitMask;
    [SerializeField] LayerMask playerLayer;

    [Header("Domain Expansion")]
    [SerializeField] GameObject domain;
    [SerializeField] Vector3 domainGravity;
    Vector3 normalGravity;

    public void Introduce()
    {
        (hp as BossHealthComponent).Show();
        (hp as BossHealthComponent).nextPhase += NextPhase;

        StartCoroutine(IntroRoutine());
    }

    protected override void Update()
    {
        if (!battleStarted) return;
        base.Update();

        // timers
        // attack checkers/counters
        // attacks
        if (attackFuncs.Count > 0 && !isAttacking && (atkTimer += Time.deltaTime) > timeBetweenAttacks)
        {
            isAttacking = true;
            atkTimer = 0;
        // pick an attack
        // start coroutine of that attack
        newAtkInd:
            int attackIndex = UnityEngine.Random.Range(0, attackFuncs.Count);
            if (attackFuncs.Count > 1 && attackIndex == lastAttackIndex && ++attackReps >= maxAttackReps)
            {
                goto newAtkInd;
            }
            StartCoroutine(attackFuncs[attackIndex]());
            lastAttackIndex = attackIndex;
        }

        // passive path
        if (followPassivePath)
        {
            // are we on path
            if (!isOnPath)
            {
                // find closest link - happens once
                if (findClosestLink)
                {
                    findClosestLink = false;
                    FindNearestPassiveLinkPos(out distToPath);
                }
                if (calculatePosAndRot)
                {
                    calculatePosAndRot = false;
                    passivePathTime = 0;
                    lookingAtQuat = Quaternion.LookRotation(passivePath.EvaluateTangent(0, pathRatio));
                    startedQuat = transform.rotation;
                    startedPos = transform.position;
                }

                // lerp to link
                transform.SetPositionAndRotation(Vector3.Lerp(startedPos, closestLinkPos, passivePathTime/distToPath),
                    Quaternion.Slerp(startedQuat, lookingAtQuat, passivePathTime));
                passivePathTime += Time.deltaTime*lerpToLinkSpeed;

                // when we reached the link - we are now on the passive path
                if (passivePathTime >= 1)
                {
                    isOnPath = true;
                    passivePathTime = 0;
                    pathRatio = targetLink / (passivePath.Splines[0].Knots.Count() * 1.0f);
                }
            }
            else
            {
                // move along path
                pathRatio += Time.deltaTime * passivePathSpeed;
                Vector3 nextpos = passivePath.EvaluatePosition(pathRatio);
                transform.LookAt(nextpos);
                transform.position = nextpos;

                // when we make full circle
                if (pathRatio >= 1)
                {
                    pathRatio = 0;
                }
            }
        }
    }

    void FindNearestPassiveLinkPos(out float dist)
    {
        closestLinkPos = passivePath.Splines [0].Knots.First().Position;
        dist = 0;
        for (int i = 0; i<passivePath.Splines[0].Knots.Count(); i++)
        {
            Vector3 linkPos = passivePath.Splines[0].Knots.ElementAt(i).Position;
            float dist1 = Vector3.Distance(transform.position, linkPos);
            dist = Vector3.Distance(transform.position, closestLinkPos);
            if (dist1<dist)
            {
                closestLinkPos = linkPos;
                targetLink = i;
            }
        }
    }

    void StartBattle()
    {
        battleStarted = true;
        target = LevelManager.PlayerTransform;
        followPassivePath = true;
        findClosestLink = true;
        calculatePosAndRot = true;
        isOnPath = false;
    }

    void NextPhase()
    {
        switch (++phase)
        {
            case 1:
                attackFuncs.Add(DiveRoutine);
                attackFuncs.Add(RamRoutine);
                attackFuncs.Add(ShootRoutine);
                break;
            case 2:
                StartCoroutine(DomainExpansionRoutine());
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    IEnumerator IntroRoutine()
    {
        if (!skipIntro)
        {
            for (float t = 0; t < 1; t += Time.deltaTime * speed)
            {
                transform.position = introPath.EvaluatePosition(t);
                transform.LookAt(introPath.EvaluatePosition(t + Time.deltaTime));
                speed = speedCurve.Evaluate(t);
                yield return null;
            }
        }

        StartBattle();
        NextPhase();
        yield break;
    }

    IEnumerator DiveRoutine()
    {
        Vector3 start, end, lerped;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        // spawn a aoe ring on the ground below the boss
        RaycastHit hit;
        Vector3 diveDir = Vector3.down + transform.forward * 0.6f;
        Debug.DrawRay(transform.position, diveDir, Color.green, Time.deltaTime);
        
        if (Physics.Raycast(transform.position, diveDir, out hit, 50f, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
        {
            aoeIndicator = Instantiate(aoeIndicatorPrefab, hit.point + Vector3.up, Quaternion.identity);
            followPassivePath = false;

            // dive into the aoe ring
            end = hit.point + Vector3.down * 40f;
            start = transform.position;
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                lerped = StaticUtilities.BuildVector(Mathf.Lerp(start.x, end.x, diveHCurve.Evaluate(t)),
                    Mathf.Lerp(start.y, end.y, diveVCurve.Evaluate(t)),
                    Mathf.Lerp(start.z, end.z, diveHCurve.Evaluate(t)));
                transform.LookAt(lerped);
                transform.position = lerped;
                yield return null;
            }
        }
        else
        {
            Debug.Log("cant find the ground for diving");
            goto skip;
        }

        // at this point, the shark is underground

        yield return new WaitForSeconds(1);

        // lerp the aoe ring to the player
        
        start = aoeIndicator.transform.position;
        end = target.position;
        for (float t = 0; t < 1; t += Time.deltaTime )
        {
            aoeIndicator.transform.position = Vector3.Lerp(start, end, curve.Evaluate(t));
            yield return null;
        }

        // boss jumps out

        transform.position = aoeIndicator.transform.position + Vector3.down * 30f;
        FindNearestPassiveLinkPos(out float dist);
        closestLinkPos += passivePath.transform.position; // passive path is at 0,0,0
        //links direction (to the next link)
        Vector3 linkDir = (Vector3)passivePath.Splines[0].Knots.ElementAt(targetLink + 1 < passivePath.Splines[0].Knots.Count()? targetLink + 1 : 0).Position - closestLinkPos;
        Destroy(aoeIndicator);

        //sharks direction to the link
        Vector3 fwd = StaticUtilities.FlatDirection(transform.position, closestLinkPos);
        float dotAngle = -0.75f;
        for (int i = targetLink + 1, cycles = 0; fwd.magnitude < 5f && Vector3.Dot(linkDir.normalized, fwd) > dotAngle; i++, cycles++)
        {
            if (cycles % passivePath.Splines[0].Knots.Count() == 0) dotAngle += 0.25f;
            if (i >= passivePath.Splines[0].Knots.Count()) i = 0;

            closestLinkPos = passivePath.Splines[0].Knots.ElementAt(i).Position;
            closestLinkPos += passivePath.transform.position; // passive path is at 0,0,0
            pathRatio = i / (passivePath.Splines[0].Knots.Count() * 1.0f);
            linkDir = (Vector3)passivePath.Splines[0].EvaluateTangent(pathRatio);
            fwd = StaticUtilities.FlatDirection(transform.position, closestLinkPos);
        }

        start = transform.position;
        end = closestLinkPos;
        for (float t = 1; t > 0; t -= Time.deltaTime)
        {
            lerped = StaticUtilities.BuildVector(Mathf.Lerp(end.x, start.x, diveHCurve.Evaluate(t)),
                Mathf.Lerp(end.y, start.y, diveVCurve.Evaluate(t)),
                Mathf.Lerp(end.z, start.z, diveHCurve.Evaluate(t)));
            transform.LookAt(lerped);
            transform.position = lerped;
            yield return null;
        }


    skip:
        isAttacking = false;
        followPassivePath = true;
        yield break;
    }

    IEnumerator RamRoutine()
    {
        // if player is too close to the walls, skip

        // go to one of corners further from the player - maybe not
        float radius = 5f;
        float frequency = 6f;
        Vector3 start, end, lerped;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        followPassivePath = false;
        // spiral to ground level
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 50f, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
        {
            end = hit.point + Vector3.up * 5f;
            start = transform.position;
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                lerped = StaticUtilities.BuildVector(start.x + radius * Mathf.Sin(t * frequency), Mathf.Lerp(start.y, end.y, curve.Evaluate(t)), start.z + radius * Mathf.Cos(t * frequency));
                transform.LookAt(lerped);
                transform.position = lerped;
                yield return null;
            }
        }
        else
        {
            Debug.Log("Ram attack cant find ground");
            goto skip;
        }

        // face the player
        Quaternion lookEnd = Quaternion.LookRotation(target.position - transform.position);
        Quaternion lookStart = transform.rotation;
       
        for (float t = 0; t < 1; t += Time.deltaTime * 2)
        {
            transform.rotation = Quaternion.Slerp(lookStart, lookEnd, curve.Evaluate(t));
            yield return null;
        }

        // aim
        float anticipationTime = 1.5f;
        for (float t = 0; t < anticipationTime; t += Time.deltaTime)
        {
            if (interrupt) goto skip;
            transform.LookAt(target);
            yield return null;
        }

        animator.SetTrigger("ChargeUp");
        yield return new WaitForSeconds(3);

        // lerp fast to player pos
        end = target.position + transform.forward * 15f;
        start = transform.position;

        for (float t = 0; t < 1; t += Time.deltaTime * 3)
        {
            transform.position = Vector3.Lerp(start, end, ramCurve.Evaluate(t));
            yield return null;
        }

        // return to passive path

        FindNearestPassiveLinkPos(out float dist);
        pathRatio = targetLink / (passivePath.Splines[0].Knots.Count() * 1.0f);
        closestLinkPos += passivePath.transform.position; // passive path is at 0,0,0
        radius = 10f;
        // end is the arc center
        start = transform.position;
        end = transform.position + transform.forward * radius + transform.up * (Vector3.Distance(start, closestLinkPos) / 2);
        lookEnd = Quaternion.LookRotation(end - start);
        lookStart = transform.rotation;
        AnimationCurve horizCuve = logCurve;
        AnimationCurve vertCuve = expCurve;
        float s = 2;

        // slerping to midpoint
        for (int i = 0; i < 2; i++)
        {

            for (float t = 0; t < 1; t += Time.deltaTime * s) 
            {
                lerped = StaticUtilities.BuildVector(Mathf.Lerp(start.x, end.x, horizCuve.Evaluate(t)),
                    Mathf.Lerp(start.y, end.y, vertCuve.Evaluate(t)),
                    Mathf.Lerp(start.z, end.z, horizCuve.Evaluate(t)));
                transform.SetPositionAndRotation(lerped, Quaternion.Slerp(lookStart, lookEnd, t));
                yield return null;
            }

            if (i == 0)
            {
                lookEnd = Quaternion.LookRotation(StaticUtilities.HorizontalizeVector(closestLinkPos - transform.position));
                lookStart = transform.rotation;
                end = closestLinkPos - (StaticUtilities.HorizontalizeVector(closestLinkPos-transform.position) * 2).normalized;
                start = transform.position;
                horizCuve = expCurve;
                vertCuve = logCurve;
            }
        }

    skip:
        followPassivePath = true;
        calculatePosAndRot = true;
        isOnPath = false;
        isAttacking = false;
        yield break;
    }

    IEnumerator ShootRoutine()
    {
        Quaternion startQuat, endQuat;
        Vector3 start, end, lerped;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        // spawn a aoe ring on the ground below the boss
        RaycastHit hit;
        Vector3 diveDir = Vector3.down + transform.forward * 0.6f;
        Debug.DrawRay(transform.position, diveDir, Color.green, Time.deltaTime);

        if (Physics.Raycast(transform.position, diveDir, out hit, 80f, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
        {
            aoeIndicator = Instantiate(aoeIndicatorPrefab, hit.point + Vector3.up, Quaternion.identity);
            followPassivePath = false;

            // dive into the aoe ring
            end = hit.point + Vector3.down * 40f;
            start = transform.position;
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                lerped = StaticUtilities.BuildVector(Mathf.Lerp(start.x, end.x, diveHCurve.Evaluate(t)),
                    Mathf.Lerp(start.y, end.y, diveVCurve.Evaluate(t)),
                    Mathf.Lerp(start.z, end.z, diveHCurve.Evaluate(t)));
                transform.LookAt(lerped);
                transform.position = lerped;
                yield return null;
            }
        }
        else
        {
            Debug.Log("cant find the ground for diving");
            goto skip;
        }

        // at this point, the shark is underground
        yield return new WaitForSeconds(0.75f);

        aoeIndicator.SetActive(false);
        
        yield return new WaitForSeconds(1);

        // tp ring to random spot
        aoeIndicator.SetActive(true);
        aoeIndicator.transform.position = LevelManager.GetRandomEnemySpawnPoint(LevelManager.Instance.CurrentEncounter.EncounterBounds);

        yield return new WaitForSeconds(2);

        // boss comes out half way out of random spot
        start = aoeIndicator.transform.position + Vector3.down * 40f;
        end = aoeIndicator.transform.position + Vector3.up * 10;
        startQuat = transform.rotation;
        endQuat = Quaternion.LookRotation(Vector3.up);
        Destroy(aoeIndicator);

        for (float t = 0; t < 1; t += Time.deltaTime * 1.5f)
        {
            transform.SetPositionAndRotation(Vector3.Lerp(start, end, curve.Evaluate(t)), Quaternion.Lerp(startQuat, endQuat, t));
            yield return null;
        }

        animator.SetBool("FacePlayer", true);

        // boss becomes a sentry turret for X amount of BIG shot
        int s = 0;
        float time = 0;
        bool canShoot = true;
        while (s < shots)
        {
            yield return null;

            transform.rotation = Quaternion.LookRotation(Vector3.up, -StaticUtilities.FlatDirection(target.position, transform.position));
            time += Time.deltaTime;

            if (time >= chargeTime && canShoot)
            {
                canShoot = false;
                time = 0;
                s++;

                // hit scan with lerp
                if (Physics.Raycast(shootOrigin.position, target.position - shootOrigin.position, out hit, 100f, hitMask, QueryTriggerInteraction.Ignore))
                {
                    Transform b = Instantiate(projectile.prefab, shootOrigin.position, Quaternion.LookRotation(target.position-shootOrigin.position)).transform;
                    b.Rotate(Vector3.right, 90f);
                    start = b.position;
                    end = hit.point;
                    float dist = Vector3.Distance(start, end);

                    for (float i = 0; i < dist; i += Time.deltaTime * projectile.speed)
                    {
                        b.position = Vector3.Lerp(start, end, i/dist);
                        yield return null;
                    }
                    b.position = end;

                    if (Physics.CheckCapsule(start,end, 1f, playerLayer, QueryTriggerInteraction.Ignore))
                    {
                        StaticUtilities.TryToDamage(target.gameObject, projectile.damage);
                        Debug.Log("hit");
                    }
                    Destroy(b.gameObject, projectile.lifeTime); 
                }
            }
            else if (time >= cooldown && !canShoot)
            {
                canShoot = true;
                time = 0;
            }
            else if (interrupt && !canShoot)
            {
                break;
            }
        }

        animator.SetBool("FacePlayer", false);
        // return to path
    skip:

        isAttacking = false;
        followPassivePath = true;
        findClosestLink = true;
        calculatePosAndRot = true;
        isOnPath = false;
        yield break;
    }

    public BossHealthComponent GetHPComponent()
    {
        return hp as BossHealthComponent;
    }

    IEnumerator DomainExpansionRoutine()
    {
        interrupt = true;
        while (isAttacking) yield return null;
        isAttacking = true;
        interrupt = false;

        // dive and appear in the middle
        Quaternion startQuat, endQuat;
        Vector3 start, end, lerped;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        // spawn a aoe ring on the ground below the boss
        RaycastHit hit;
        Vector3 diveDir = Vector3.down + transform.forward * 0.6f;
        Debug.DrawRay(transform.position, diveDir, Color.green, Time.deltaTime);

        if (Physics.Raycast(transform.position, diveDir, out hit, 80f, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
        {
            followPassivePath = false;

            // dive into the aoe ring
            end = hit.point + Vector3.down * 40f;
            start = transform.position;
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                lerped = StaticUtilities.BuildVector(Mathf.Lerp(start.x, end.x, diveHCurve.Evaluate(t)),
                    Mathf.Lerp(start.y, end.y, diveVCurve.Evaluate(t)),
                    Mathf.Lerp(start.z, end.z, diveHCurve.Evaluate(t)));
                transform.LookAt(lerped);
                transform.position = lerped;
                yield return null;
            }
        }
        else
        {
            Debug.Log("cant find the ground for diving");
            goto skip;
        }

        // at this point, the shark is underground
        yield return new WaitForSeconds(1);

        // boss comes out half way out of random spot
        start = LevelManager.Instance.CurrentEncounter.EncounterBounds.center + Vector3.down * 50f;
        end = LevelManager.Instance.CurrentEncounter.EncounterBounds.center + Vector3.up * 6;
        startQuat = transform.rotation;
        endQuat = Quaternion.LookRotation(Vector3.up);

        for (float t = 0; t < 1; t += Time.deltaTime * 1.5f)
        {
            transform.SetPositionAndRotation(Vector3.Lerp(start, end, curve.Evaluate(t)), Quaternion.Lerp(startQuat, endQuat, t));
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        Vector3 domainTrueScale = Vector3.one * 130f;

        // Spawn domain
        domain.SetActive(true);
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            domain.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one*2, curve.Evaluate(t));
            yield return null;
        }
        yield return new WaitForSeconds(1);
        for (float t = 0; t < 1; t+=Time.deltaTime)
        {
            domain.transform.localScale = Vector3.Lerp(Vector3.one * 2, domainTrueScale, curve.Evaluate(t));
            yield return null;
        }

        // gravity effects
        // slow player?
        normalGravity = Physics.gravity;
        Physics.gravity = domainGravity;

    skip:

        isAttacking = false;
        followPassivePath = true;
        findClosestLink = true;
        calculatePosAndRot = true;
        isOnPath = false;
        yield break;
    }
}
