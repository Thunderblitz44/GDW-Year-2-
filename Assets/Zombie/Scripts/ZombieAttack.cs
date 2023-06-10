using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    // damage applier
    [SerializeField] float damage = 0f;
    [SerializeField] float reach = 1.5f;
    [SerializeField] float angle = 90f;
    [SerializeField] float rate = 1f;

    bool canAttack = false;

    public void Attack()
    {

    }

    public void StopAttack()
    {

    }

}
