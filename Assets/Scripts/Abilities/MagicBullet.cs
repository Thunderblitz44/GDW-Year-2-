using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    public ProjectileData Projectile { get; private set; }
    public Rigidbody Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Die();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Projectile.owner && Projectile.owner.gameObject == collision.gameObject) return;

        if (!StaticUtilities.TryToDamage(collision.collider.gameObject, Projectile.damage))
            StaticUtilities.TryToDamage(collision.gameObject, Projectile.damage);
        CancelInvoke(nameof(Die));
        Die();
    }

    private void OnEnable()
    {
        Invoke(nameof(Die), Projectile.lifeTime);
    }

    void Die()
    {
        Rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void Initialize(ProjectileData data)
    {
        Projectile = data;
        Rb.excludeLayers = data.ignoreLayers;
        GetComponent<SphereCollider>().excludeLayers = data.ignoreLayers;
    }
}
