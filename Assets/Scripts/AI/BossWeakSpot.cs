using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeakSpot : DamageableEntity
{
    internal override void Awake()
    {
        base.Awake();
        hp = GetComponentInParent<BossHealthComponent>();
    }
}
