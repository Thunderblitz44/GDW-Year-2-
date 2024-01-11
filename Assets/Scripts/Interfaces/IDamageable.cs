
public interface IDamageable
{
    public void ApplyDamage(float damage, DamageTypes type);

    public void ApplyDamageOverTime(float damage, DamageTypes type, float duration);
}