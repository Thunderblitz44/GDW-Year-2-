using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyAnimator : MonoBehaviour
{
    Animator animator;

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
