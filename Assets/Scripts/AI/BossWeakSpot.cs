
public class BossWeakSpot : DamageableEntity
{
    float stunTime = 5;

    protected override void Awake()
    {
        base.Awake();
        hp = GetComponentInParent<BossHealthComponent>();
    }

    public void Stun()
    {
        isInvincible = false;
        ApplyDamage(10);
        Invoke(nameof(BecomeInvincible), stunTime);
    }

    void BecomeInvincible()
    {
        isInvincible = true;
    }
}
