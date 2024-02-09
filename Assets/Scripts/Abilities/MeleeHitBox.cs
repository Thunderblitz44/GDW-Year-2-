using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    SphereCollider sc;
    public int Damage { get; set; }
    public Vector2 Knockback { get; set; }

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
        StaticUtilities.TryToDamage(other.gameObject, Damage);

        Rigidbody rb;
        if (other.gameObject.TryGetComponent(out rb))
        {
            rb.AddForce((transform.forward * Knockback.x + Vector3.up * Knockback.y) * rb.mass, ForceMode.Impulse);
        }

        CancelInvoke(nameof(Hide));
        gameObject.SetActive(false);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
