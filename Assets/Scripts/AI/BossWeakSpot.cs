using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeakSpot : DamageableEntity
{
    float stunTime = 5;
    bool wasInvincible = false;

    protected override void Awake()
    {
        base.Awake();
        hp = GetComponentInParent<BossHealthComponent>();
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);
        if (wasInvincible && !isInvincible)
        {
            Invoke(nameof(BecomeInvincible), stunTime);
        }
        wasInvincible = isInvincible;
    }

    void BecomeInvincible()
    {
        isInvincible = true;
        Debug.Log("unstun");
    }

}
