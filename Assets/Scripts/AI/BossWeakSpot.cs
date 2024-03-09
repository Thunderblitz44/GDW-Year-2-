using UnityEngine;

public class BossWeakSpot : DamageableEntity
{
    Enemy bossScript;
    [SerializeField] bool flashThis = false;
    float flashTimer = 0;
    MeshRenderer mr;


    protected override void Awake()
    {
        base.Awake();
        hp = GetComponentInParent<BossHealthComponent>();
        bossScript = GetComponentInParent<GolemBossScript>();
        if (!bossScript) bossScript = GetComponentInParent<SharkBoss>();
        mr = GetComponentInChildren<MeshRenderer>();
    }

    private void Update()
    {
        if (flashThis && flashTimer > 0 && mr)
        {
            flashTimer -= Time.deltaTime;
            float flashIntensity = Mathf.Lerp(0f, 1f, flashTimer / StaticUtilities.damageFlashDuration);
            mr.materials[0].SetFloat("_flash", flashIntensity);
        }
    }

    public void Stun(int damage)
    {
        ApplyDamage(damage);
        if (bossScript.GetType().Equals(typeof(GolemBossScript))) (bossScript as GolemBossScript).Stun();
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);
        if (bossScript.GetType().Equals(typeof(SharkBoss))) (bossScript as SharkBoss).flashTimer = 0.2f;
        if (flashThis && !isInvincible) flashTimer = StaticUtilities.damageFlashDuration;
    }
}
