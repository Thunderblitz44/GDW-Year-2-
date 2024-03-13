using UnityEngine;
using UnityEngine.AI;

public class Enemy : DamageableEntity
{
    protected Transform target;
    protected NavMeshAgent agent;
    [SerializeField] float slowUpdateInterval = 0.2f;
    protected bool updateTargetOnDamaged = true;
    float updateTimer;

    protected Animator animator;
    protected SkinnedMeshRenderer skinnedMeshRenderer;
    [HideInInspector] public float flashTimer = 0f; 
    [HideInInspector] public float dissolveTimer = 0f; 
    private bool dissolve = false;
    private float dissolveTime = 0f;
    private float dissolveSpeed = 0.3f;
    private bool isAwake = true;
    public float spawndissolveTimer = 1f;
    protected override void Awake()
    {
        base.Awake();
        
        animator = GetComponent<Animator>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        agent = GetComponent<NavMeshAgent>();
        target = LevelManager.PlayerTransform;
        if (skinnedMeshRenderer) skinnedMeshRenderer.material.SetFloat("_dissolve", 1);
    }

    protected virtual void Update()
    {
        if (!target) target = LevelManager.PlayerTransform;
        if (!target) return;

        // timer to recalculate navmesh agent
        updateTimer += Time.deltaTime;
        if (updateTimer >= slowUpdateInterval) SlowUpdate();
        
        if (skinnedMeshRenderer && flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            float flashIntensity = Mathf.Lerp(0f, 1f, flashTimer / StaticUtilities.damageFlashDuration);
            skinnedMeshRenderer.material.SetFloat("_flash", flashIntensity);
        }

        if (skinnedMeshRenderer && isAwake)
        {
            spawndissolveTimer = Mathf.Lerp(1f, 0f, Time.time );
           
            skinnedMeshRenderer.material.SetFloat("_dissolve", spawndissolveTimer);

        }
        if ( skinnedMeshRenderer && dissolve)
        {
            
            dissolveTimer += Time.deltaTime * dissolveSpeed * 2;

          
          
        
            skinnedMeshRenderer.material.SetFloat("_dissolve", dissolveTimer);

         
        }
    }

    protected override void OnHealthZeroed()
    {
        dissolve = true;
        Destroy(agent);
        Destroy(GetComponent<LockonTarget>());
        base.OnHealthZeroed();
    }

    protected virtual void SlowUpdate()
    {
        updateTimer = 0;
        if (!agent || !agent.isActiveAndEnabled) return;
        agent.SetDestination(target.position);
    }

    public void SetFollowTarget(Transform target)
    {
        this.target = target;
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);
      
        // Trigger flash effect
        if (!isInvincible) flashTimer = StaticUtilities.damageFlashDuration;
        if (updateTargetOnDamaged) target = LevelManager.PlayerTransform;
    }

    
}
