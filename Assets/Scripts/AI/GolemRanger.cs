using UnityEngine;
using UnityEngine.VFX;

public class GolemRanger : Enemy
{
    // attack
    [SerializeField] ProjectileData projectile = ProjectileData.defaultProjectile;
    [SerializeField] Transform shootOrigin;
    [SerializeField] new ParticleSystem particleSystem;
    [SerializeField] GameObject HeadTarget;
    AttackTrigger trigger;
    private float xSpeed;
    private float zSpeed;
    public float shootForce = 5;
    public ParticleSystem DustSystemRight;
    public ParticleSystem DustSystemLeft;
    private int DeathType;
    Vector3 localVelocity;
    public VisualEffect vfxGraph;

    protected override void Awake()
    {
        base.Awake();

        particleSystem.GetComponent<MagicBullet>().Initialize(projectile, this);
        int randomDeathType = Random.Range(0, 2);
        DeathType = randomDeathType;
        animator.SetInteger("deathType", randomDeathType);
        if (!shootOrigin) 
        { 
            Debug.LogWarning("No shoot origin set for ranger golem!");
            shootOrigin = transform;
        }

        trigger = transform.GetComponentInChildren<AttackTrigger>();
        if (trigger)
        {
            trigger.onTriggerEnter += OnAttackTriggerEnter;
            trigger.onTriggerExit += OnAttackTriggerExit;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!target) return;

        HeadTarget.transform.position = target.position;

        float smoothingFactor = 0.1f;

        if (agent) localVelocity = transform.InverseTransformDirection(agent.velocity.normalized);
        else localVelocity = Vector3.zero;

        // Smooth the velocity components (remove the float keyword)
        xSpeed = Mathf.Lerp(xSpeed, localVelocity.x, smoothingFactor);
        zSpeed = Mathf.Lerp(zSpeed, localVelocity.z, smoothingFactor);
        // Set the velocity values in the animator
        animator.SetFloat("XSpeed", xSpeed);
        animator.SetFloat("ZSpeed", zSpeed);
    }

    void OnAttackTriggerEnter(Collider other)
    {
        //attack = true;
        animator.SetBool("InAttackRange", true);
    }

    void OnAttackTriggerExit(Collider other)
    {
        animator.SetBool("InAttackRange", false);
    }

    protected override void OnHealthZeroed()
    {
        base.OnHealthZeroed();
        particleSystem.Stop(); 
    }

    public void EnableAI()
    {
        if (agent) agent.enabled = true;
        HeadTarget.SetActive(true);
    }

    public void DisableAI()
    {
        if (agent) agent.enabled = false;
        HeadTarget.SetActive(false);
    }
    
    public void DustLeft()
    {
        DustSystemLeft.Emit(6);
    }
    
    public void DustRight()
    {
        DustSystemRight.Emit(6);
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void DeathBurst()
    {
        vfxGraph.SendEvent("death");
    }
}
