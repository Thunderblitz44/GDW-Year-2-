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
    public bool Destroy { get; private set; }

    public ProjectileData(int damage, float lifeTime, float speed, LayerMask ignoreLayers, DamageableEntity owner)
    {
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.speed = speed;
        this.owner = owner;
        this.ignoreLayers = ignoreLayers;
        prefab = null;
        Destroy = false;
    }

    public void CheckPrefab()
    {
        if (prefab) return;
#if UNITY_EDITOR
        prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Magic Bullet/MagicBullet.prefab");
        if (owner) Debug.LogWarningFormat("Make sure to assign projectile prefab for {0}", owner);
        else Debug.LogWarningFormat("Make sure to assign projectile prefabs");
        if (!prefab) Debug.LogWarning("Can't find MagicBullet in Assets/Prefabs/Magic Bullet/");
#endif
    }

    public void OwnerDestroyed()
    {
        Destroy = true;
    }
}