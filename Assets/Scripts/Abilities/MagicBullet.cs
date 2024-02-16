using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    public ProjectileData Projectile { get; private set; }
    public Rigidbody Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        if (Rb) Die();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((Projectile.owner && Projectile.owner.gameObject == collision.gameObject) ||
            (Projectile.owner.gameObject.layer == LayerMask.NameToLayer("Player") && collision.gameObject.layer == LayerMask.NameToLayer("Friendly"))) return;

        if (!StaticUtilities.TryToDamage(collision.collider.gameObject, Projectile.damage))
            StaticUtilities.TryToDamage(collision.gameObject, Projectile.damage);
        CancelInvoke(nameof(Die));
        Die();
    }

    private void OnEnable()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Elana Shoot", gameObject);
        if (Rb) Invoke(nameof(Die), Projectile.lifeTime);
    }

    void Die()
    {
        if (Rb) Rb.velocity = Vector3.zero;
        if (!Projectile.Destroy) gameObject.SetActive(false);
        else Destroy(gameObject);
    }

    public void Initialize(ProjectileData data)
    {
        Initialize(data, data.owner);
    }

    public void Initialize(ProjectileData data, DamageableEntity owner)
    {
        Projectile = data;
        if (Rb)
        {
            Rb.excludeLayers = data.ignoreLayers;
            GetComponent<SphereCollider>().excludeLayers = data.ignoreLayers;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if ((Projectile.owner && Projectile.owner.gameObject == other) ||
            (Projectile.owner.gameObject.layer == LayerMask.NameToLayer("Player") && other.layer == LayerMask.NameToLayer("Friendly"))) return;
        StaticUtilities.TryToDamage(other, Projectile.damage);
    }
}
