using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : DamageableEntity
{
    [Header("Movement")]
    Transform target;
    NavMeshAgent agent;
    [SerializeField] float destinationRecalculationInterval = 0.2f;
    float updateTimer;
    bool targetReached;

    [Header("Behaviour")]
    [SerializeField] float detectionRange = 20f;
    bool playerDetected = false;

    /*[Header("Attack")]
    [SerializeField] float damage = 0f;
    [SerializeField] float attackDelay = 1;
    [SerializeField] GameObject projectilePrefab;
    float time;
    bool isAttacking;
    bool canAttack;*/

    [Header("Animations")]
    [SerializeField] Animator animator;
    string currentAnimation;


    internal override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!agent) return;


        /*time += Time.deltaTime;
        if (time < attackDelay) return;

        if (canAttack && !IsAnimationPlaying(1))
        {
            AttackLoop();
        }
        else if (isAttacking)
        {
            time = 0;
            isAttacking = false;
        }*/


        // timer to recalculate navmesh agent
        updateTimer += Time.deltaTime;
        if (updateTimer < destinationRecalculationInterval)
        {
            return;
        }

        SlowUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerDetected = true;
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerDetected = false;
            target = null;
        }
    }

    internal virtual void SlowUpdate()
    {
        updateTimer = 0;
        if (!target) return;

        // go to target
        agent.SetDestination(target.position);

        float distance = Vector3.Distance(transform.position, target.position);

        // reached attacking distance
        if (distance < agent.stoppingDistance && !targetReached)
        {
            targetReached = true;
        }
        // target is fleeing. Can we attack? Is the target outside of our range?
        else if (agent.isStopped && targetReached && distance >= agent.stoppingDistance)
        {
            targetReached = false;
        }
    }

    public virtual void SetFollowTarget(Transform target)
    {
        this.target = target;
    }

    /*internal virtual void AttackLoop()
    {
        isAttacking = true;
        PlayAnimation(attackAnimation);
    }*/

    public bool IsAnimationPlaying(int layer)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1;
    }

    internal void ChangeAnimation(string newAnimation)
    {
        if (!animator) return;

        if (newAnimation == currentAnimation) return;
        PlayAnimation(newAnimation);
    }

    internal void PlayAnimation(string animation)
    {
        if (!animator) return;

        animator.Play(animation);
        currentAnimation = animation;
    }
}
