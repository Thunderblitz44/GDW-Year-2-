using UnityEngine;


public class MeleeHitBox : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public Vector2 knockback;
   
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb;
        SkinnedMeshRenderer skinnedMeshRenderer;
        if (other.gameObject.TryGetComponent(out rb))
        {
            rb.AddForce((transform.forward * knockback.x + Vector3.up * knockback.y) * rb.mass, ForceMode.Impulse);
        }
        if (StaticUtilities.TryToDamage(other.gameObject, damage))
        {
            gameObject.SetActive(false);
        }
        if (other.gameObject.TryGetComponent(out skinnedMeshRenderer))
        {
            skinnedMeshRenderer.material.SetFloat("flash", 1f);
        }
        //CancelInvoke(nameof(Hide));
        //gameObject.SetActive(false);
    }

    private void OnParticleCollision(GameObject other)
    {
        Rigidbody rb;
        SkinnedMeshRenderer skinnedMeshRenderer;
        if (other.gameObject.TryGetComponent(out rb ))
        {
            Vector3 dir = StaticUtilities.FlatDirection(other.transform.position, transform.position);
            rb.AddForce((dir * knockback.x + Vector3.up * knockback.y) * rb.mass, ForceMode.Impulse);
        }

        if (other.gameObject.TryGetComponent(out skinnedMeshRenderer))
        {
            skinnedMeshRenderer.material.SetFloat("flash", 1f);
        }
        StaticUtilities.TryToDamage(other.gameObject, damage);
    }
}
