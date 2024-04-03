using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBurst : MonoBehaviour
{
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    private ParticleSystem WindBurstSystem;
    // Start is called before the first frame update
    void Start()
    {
        WindBurstSystem = GetComponent<ParticleSystem>();
        WindBurstSystem.GetComponent<MagicBullet>().Initialize(projectile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Burst()
    {
        WindBurstSystem.Emit(1);
    }
}
