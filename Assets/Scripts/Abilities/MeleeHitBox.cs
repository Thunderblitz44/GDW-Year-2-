using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeHitBox : MonoBehaviour
{
    SphereCollider sc;
    public int damage;
    [HideInInspector] public Vector2 knockback;

    private void Awake()
    {
        sc = GetComponent<SphereCollider>();
    }

   

    private void OnTriggerEnter(Collider other)
    {
        StaticUtilities.TryToDamage(other.gameObject, damage);

        Rigidbody rb;
        if (other.gameObject.TryGetComponent(out rb))
        {
            rb.AddForce((transform.forward * knockback.x + Vector3.up * knockback.y) * rb.mass, ForceMode.Impulse);
        }

        CancelInvoke(nameof(Hide));
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ReadyAttack()
    {
        gameObject.SetActive(true);
    }

   
 
}
