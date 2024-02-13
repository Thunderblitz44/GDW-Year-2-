using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeHitBox : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public Vector2 knockback;
   
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb;
        if (other.gameObject.TryGetComponent(out rb))
        {
            rb.AddForce((transform.forward * knockback.x + Vector3.up * knockback.y) * rb.mass, ForceMode.Impulse);
        }
        if (StaticUtilities.TryToDamage(other.gameObject, damage))
        {
            Hide();
        }

        //CancelInvoke(nameof(Hide));
        //gameObject.SetActive(false);
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
