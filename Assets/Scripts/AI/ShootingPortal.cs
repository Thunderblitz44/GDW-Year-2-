using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShootingPortal : MonoBehaviour
{
    int shots;
    int shotCount;
    ProjectileData projectile;
    float shotCooldown;
    float shotTimer;
    float startDelay;
    float startTimer;
    readonly List<GameObject> pooledProjectiles = new(4);
    bool settingUp = true;

    public void Setup(ProjectileData projectile, int shots, float shotCooldown, float startDelay)
    {
        this.projectile = projectile;
        this.shots = shots;
        this.shotCooldown = shotCooldown;
        this.startDelay = startDelay;
        if (!projectile.prefab) projectile.prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Magic Bullet/MagicBullet.prefab");

        for (int i = 0; i < pooledProjectiles.Capacity; i++)
        {
            MagicBullet mb = Instantiate(projectile.prefab).GetComponent<MagicBullet>();
            mb.Initialize(projectile);
            pooledProjectiles.Add(mb.gameObject);
        }
        settingUp = false;
    }

    private void OnDisable()
    {
        startTimer = 0;
        shotCount = 0;
    }

    private void Update()
    {
        if (settingUp) return; 

        startTimer += Time.deltaTime;
        if (startTimer > startDelay && (shotTimer += Time.deltaTime) > shotCooldown) 
        {
            shotTimer = 0;
            Vector3 force = (LevelManager.PlayerTransform.position - transform.position).normalized * projectile.speed;
            StaticUtilities.ShootProjectile(pooledProjectiles, transform.position, force);
            if (++shotCount >= shots) gameObject.SetActive(false);
        }

        transform.LookAt(LevelManager.PlayerTransform);
    }

    /*private void OnDestroy()
    {
        foreach (GameObject mb in pooledProjectiles)
        {
            mb.GetComponent<MagicBullet>().Projectile.OwnerDestroyed();
        }
    }*/
}
