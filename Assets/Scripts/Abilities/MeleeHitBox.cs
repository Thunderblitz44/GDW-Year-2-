using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    SphereCollider sc;
    [HideInInspector] public float damage;
    [HideInInspector] public Vector2 knockback;

    private void Awake()
    {
        sc = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        Invoke(nameof(Hide), 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable d;
        if (other.gameObject.TryGetComponent(out d))
        {
            d.ApplyDamage(1, DamageTypes.physical);
        }

        Rigidbody rb;
        if (other.gameObject.TryGetComponent(out rb))
        {
            rb.AddForce((transform.forward * knockback.x + Vector3.up * knockback.y) * rb.mass, ForceMode.Impulse);
        }

        CancelInvoke(nameof(Hide));
        gameObject.SetActive(false);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
