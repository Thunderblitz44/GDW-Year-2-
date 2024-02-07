using System;
using UnityEngine;

[Serializable]
public struct ProjectileData
{
    public static ProjectileData defaultProjectile = new(1f, 1f, 60f, null);

    public float damage;
    public float lifeTime;
    public float speed;
    public GameObject prefab;
    [HideInInspector] public DamageableEntity owner;

    public ProjectileData(float damage, float lifeTime, float speed, DamageableEntity owner)
    {
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.speed = speed;
        this.owner = owner;
        prefab = null;
    }
}