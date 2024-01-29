using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GolemBossScript : Enemy
{
    [Header("Attack Pattern")]
    [SerializeField] int minAtkReps = 1;
    [SerializeField] int maxAtkReps = 5;
    int atkRepetitions;
    int atkCounter;

    [Header("Portal Projectiles")]
    [SerializeField] int shots;
    [SerializeField] int portals;
    [SerializeField] float shotDelay;
    [SerializeField] float shotSpeed;
    [SerializeField] float shotDamage;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject portalPrefab;

    [Header("Lasers")]
    [SerializeField] float laserDamage;

    internal override void Update()
    {
    }
}
