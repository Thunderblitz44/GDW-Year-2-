using UnityEngine;
using UnityEngine.VFX;

public class MagicBullet : MonoBehaviour
{
    public ProjectileData Projectile { get; private set; }
    public Rigidbody Rb { get; protected set; }
    public GameObject decalPrefab;

    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
       
        if (Rb) Die();
      
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!StaticUtilities.TryToDamage(collision.collider.gameObject, Projectile.damage))
            StaticUtilities.TryToDamage(collision.gameObject, Projectile.damage);
        CancelInvoke(nameof(Die));
        Die();
    }

    void OnEnable()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Elana Shoot", gameObject);
        if (Rb) Invoke(nameof(Die), Projectile.lifeTime);
    
    }

    void Die()
    {
      
            if (Rb) Rb.velocity = Vector3.zero;
            if (!Projectile.Destroy) Decal();
            else Destroy(gameObject);
      
     
    }
    void ProcessHitEffects()
    {
        if (!Projectile.Destroy)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
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
            GetComponent<Collider>().excludeLayers = data.ignoreLayers;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if ((Projectile.owner && Projectile.owner.gameObject == other) ||
            (Projectile.owner.gameObject.layer == LayerMask.NameToLayer("Player") && other.layer == LayerMask.NameToLayer("Friendly"))) return;
        StaticUtilities.TryToDamage(other, Projectile.damage);
    }

    private void Decal()
    {
        GameObject decalInstance = Instantiate(decalPrefab, transform.position, Quaternion.identity);
        
        // Calculate the rotation to face towards the collision point
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            Vector3 hitPoint = hit.point;
            Vector3 objectPosition = transform.position;
            Vector3 directionToHit = hitPoint - objectPosition;
            Quaternion rotationToHit = Quaternion.LookRotation(directionToHit);

            decalInstance.transform.rotation = rotationToHit;
        }

        gameObject.SetActive(false);
        Destroy(decalInstance, 1f); 
    }

    }
    

