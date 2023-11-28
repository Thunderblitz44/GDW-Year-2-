using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    EnemyAnimator animationHandler;
    EnemyBehaviours behaviourHandler;
    EnemyDetection detectionHandler;
    EnemyMovement movementHandler;
    EnemyAttack attackHandler;

    List<Transform> detectedPlayers = new();

    private void Start()
    {
        animationHandler = GetComponent<EnemyAnimator>();
        behaviourHandler = GetComponent<EnemyBehaviours>();
        detectionHandler = GetComponent<EnemyDetection>();
        movementHandler = GetComponent<EnemyMovement>();
        attackHandler = GetComponent<EnemyAttack>();


        if (behaviourHandler)
        {
            behaviourHandler.onBehaviourChanged += OnBehaviourChanged;
        }

        if (detectionHandler)
        {
            detectionHandler.onPlayerDetected += OnPlayerDetected;
            detectionHandler.onPlayerLost += OnPlayerLost;
        }

        if (movementHandler)
        {
            movementHandler.onAttackDistanceReached += OnAttackDistanceReached;
            movementHandler.onTargetFled += OnTargetFled;
        }
    }

    public override void OnDestroy()
    {
        if (behaviourHandler)
        {
            behaviourHandler.onBehaviourChanged -= OnBehaviourChanged;
        }

        if (detectionHandler)
        {
            detectionHandler.onPlayerDetected -= OnPlayerDetected;
            detectionHandler.onPlayerLost -= OnPlayerLost;
        }

        if (movementHandler)
        {
            movementHandler.onAttackDistanceReached -= OnAttackDistanceReached;
            movementHandler.onTargetFled -= OnTargetFled;
        }
        base.OnDestroy();
    }

    void OnPlayerDetected(object sender, Transform player)
    {
        detectedPlayers.Add(player);

        behaviourHandler?.ChangeBehaviour(EnemyBehaviours.Behaviour.following);
    }

    void OnPlayerLost(object sender, Transform player)
    {
        detectedPlayers.Remove(player);

        behaviourHandler?.ChangeBehaviour(EnemyBehaviours.Behaviour.passive);
    }

    void OnBehaviourChanged(object sender, EnemyBehaviours.Behaviour behaviour)
    {
        switch (behaviour)
        {
            case EnemyBehaviours.Behaviour.passive:
                SetPassive();
                break;
            case EnemyBehaviours.Behaviour.following:
                SetFollowing();
                break;
            case EnemyBehaviours.Behaviour.aggressive_chase:
                SetAggressiveChase();
                break;
            case EnemyBehaviours.Behaviour.attacking:
                SetAttacking();
                break;
            case EnemyBehaviours.Behaviour.dying:
                SetDying();
                break;
            default:
                break;
        }
    }

    void SetPassive()
    {
        // passive commands
        //animationHandler?.ChangeAnimation(StaticUtilities.ZOMBIE_IDLE);

        //movementHandler?.SetFollowTarget(detectedPlayers[0]);
    }

    void SetFollowing()
    {
        //animationHandler?.ChangeAnimation(StaticUtilities.ZOMBIE_WALK);

        movementHandler?.SetFollowTarget(detectedPlayers[0]);
    }

    void SetAggressiveChase()
    {
    }

    void SetAttacking()
    {

    }

    void SetDying()
    {

    }


    void OnAttackDistanceReached()
    {
        attackHandler?.Attack();
        //behaviourHandler?.ChangeBehaviour(EnemyBehaviours.Behaviour.attacking);
        //animationHandler?.ChangeAnimation(StaticUtilities.ZOMBIE_ATTACK);
    }

    void OnTargetFled()
    {
        attackHandler?.StopAttack();
        //behaviourHandler?.ChangeBehaviour(EnemyBehaviours.Behaviour.attacking);
    }

    public List<Transform> GetDetectedPlayers() => detectedPlayers;
}