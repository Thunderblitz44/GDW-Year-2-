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
            gameObject.SetActive(false);
        }

        //CancelInvoke(nameof(Hide));
        //gameObject.SetActive(false);
    }

    private void OnParticleCollision(GameObject other)
    {
        Rigidbody rb;
        if (other.gameObject.TryGetComponent(out rb))
        {
            Vector3 dir = StaticUtilities.FlatDirection(other.transform.position, transform.position);
            rb.AddForce((dir * knockback.x + Vector3.up * knockback.y) * rb.mass, ForceMode.Impulse);
        }
        StaticUtilities.TryToDamage(other.gameObject, damage);
    }
}
