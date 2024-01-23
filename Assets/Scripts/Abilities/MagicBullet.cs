using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    [HideInInspector]public float damage = 1;
    [HideInInspector]public float lifetime = 1;
    [HideInInspector]public DamageableEntity owner;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.SetActive(false);
        Invoke(nameof(Die), lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == owner.gameObject) return;

        IDamageable d;
        if (collision.gameObject.TryGetComponent(out d))
        {
            d.ApplyDamage(damage, DamageTypes.magic);
        }
        CancelInvoke(nameof(Die));
        Die();
    }

    void Die()
    {
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Invoke(nameof(Die), lifetime);
    }
}
