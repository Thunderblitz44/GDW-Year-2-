using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;

    string currentAnimation;

    public bool isAnimationPlaying(int layer)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1;
    }

    public void ChangeAnimation(string newAnimation)
    {
        if (!animator) return;

        if (newAnimation == currentAnimation) return;
        PlayAnimation(newAnimation);
    }

    public void PlayAnimation(string animation)
    {
        if (!animator) return;

        animator.Play(animation);
        currentAnimation = animation;
    }
}
