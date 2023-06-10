using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Zombie))]
public class ZombieAnimator : MonoBehaviour
{
    Animator animator;

    //* Animation Keywords *//
    public const string ZOMBIE_IDLE = "Zombie Idle";
    public const string ZOMBIE_WALK = "Zombie Walk";
    public const string ZOMBIE_RUN = "Zombie Run";
    public const string ZOMBIE_CRAWL = "Zombie Crawl";
    public const string ZOMBIE_ATTACK = "Zombie Attack";

    string currentAnimation;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangeAnimation(string newAnimation)
    {
        if (newAnimation == currentAnimation) return;
        PlayAnimation(newAnimation);
    }

    void PlayAnimation(string animation)
    {
        if (!animator) return;

        animator.Play(animation);
        currentAnimation = animation;
    }
}
