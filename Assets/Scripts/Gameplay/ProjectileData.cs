using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct ProjectileData
{
    public static ProjectileData defaultProjectile = new(1, 1f, 60f, 0, null);

    public int damage;
    public float lifeTime;
    public float speed;
    public GameObject prefab;
    public LayerMask ignoreLayers;
    public DamageableEntity owner;

    public ProjectileData(int damage, float lifeTime, float speed, LayerMask ignoreLayers, DamageableEntity owner)
    {
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.speed = speed;
        this.owner = owner;
        this.ignoreLayers = ignoreLayers;
        prefab = null;
    }

    public void CheckPrefab()
    {
        if (prefab) return;
        prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Magic Bullet/MagicBullet.prefab");
        if (!prefab) Debug.LogWarning("Can't find MagicBullet in Assets/Prefabs/Magic Bullet/");
    }
}