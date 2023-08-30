using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void ApplyDamage(float damage);

    public void ApplyDamageOverTime(float damage, float duration);
}