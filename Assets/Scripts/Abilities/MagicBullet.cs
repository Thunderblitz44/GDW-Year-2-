using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    public ProjectileData Projectile { get; set; }
    public Rigidbody Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        gameObject.SetActive(false);
        Invoke(nameof(Die), Projectile.lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == Projectile.owner.gameObject) return;

        IDamageable d;
        if (collision.gameObject.TryGetComponent(out d))
        {
            d.ApplyDamage(Projectile.damage, DamageTypes.magic);
        }
        CancelInvoke(nameof(Die));
        Die();
    }

    void Die()
    {
        Rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Invoke(nameof(Die), Projectile.lifeTime);
    }
}
