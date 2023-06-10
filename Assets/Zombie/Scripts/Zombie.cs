using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] ZombieAnimator animationHandler;
    [SerializeField] ZombieBehaviours behaviourHandler;
    [SerializeField] ZombieDetection detectionHandler;
    [SerializeField] ZombieMovement movementHandler;
    [SerializeField] ZombieAttack attackHandler;

    Transform detectedPlayer;

    private void Start()
    {
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
            movementHandler.onTargetReached += OnTargetReached;
        }
    }

    void OnPlayerDetected(object sender, Transform player)
    {
        detectedPlayer = player;

        behaviourHandler?.ChangeBehaviour(ZombieBehaviours.Behaviour.following);
    }

    void OnPlayerLost()
    {
        detectedPlayer = null;

        behaviourHandler?.ChangeBehaviour(ZombieBehaviours.Behaviour.passive);
    }

    void OnBehaviourChanged(object sender, ZombieBehaviours.Behaviour behaviour)
    {
        switch (behaviour)
        {
            case ZombieBehaviours.Behaviour.passive:
                SetPassive();
                break;
            case ZombieBehaviours.Behaviour.following:
                SetFollowing();
                break;
            case ZombieBehaviours.Behaviour.aggressive_chase:
                SetAggressiveChase();
                break;
            case ZombieBehaviours.Behaviour.dying:
                SetDying();
                break;
            default:
                break;
        }
    }

    void SetPassive()
    {
        // passive commands
        animationHandler?.ChangeAnimation(ZombieAnimator.ZOMBIE_IDLE);

        movementHandler?.SetFollowTarget(detectedPlayer);
    }

    void SetFollowing()
    {
        animationHandler?.ChangeAnimation(ZombieAnimator.ZOMBIE_WALK);

        movementHandler?.SetFollowTarget(detectedPlayer);
    }

    void SetAggressiveChase()
    {

    }

    void SetDying()
    {

    }


    void OnTargetReached()
    {
        attackHandler?.Attack();
        animationHandler?.ChangeAnimation(ZombieAnimator.ZOMBIE_ATTACK);
    }

    void OnTargetFled()
    {
        attackHandler?.StopAttack();

    }

    public Transform GetDetectedPlayer() => detectedPlayer;
}
