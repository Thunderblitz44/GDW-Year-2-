
public class BossWeakSpot : DamageableEntity
{
    GolemBossScript bossScript;

    protected override void Awake()
    {
        base.Awake();
        hp = GetComponentInParent<BossHealthComponent>();
        bossScript = GetComponentInParent<GolemBossScript>();
    }

    public void Stun(int damage)
    {
        ApplyDamage(damage);
        bossScript.Stun();
    }
}
