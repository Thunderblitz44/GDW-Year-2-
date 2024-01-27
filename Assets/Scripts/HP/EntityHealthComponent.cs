using UnityEngine;

public class EntityHealthComponent : HealthComponent
{
    [SerializeField] internal float height = 1.5f;
    EntityHPBar entityHPBar;

    private void Awake()
    {
        health = maxHealth;

        entityHPBar = Instantiate(hpBarPrefab, LevelManager.Instance.WorldCanvas).GetComponent<EntityHPBar>();
        entityHPBar.transform.position = transform.position + Vector3.up * height;
        entityHPBar.maxHP = maxHealth;
        entityHPBar.SetHPValue(health);
    }

    private void Update()
    {
        if (!entityHPBar) return;
        entityHPBar.transform.position = transform.position + Vector3.up * height;
    }

    public override void DeductHealth(float value)
    {
        health = Mathf.Clamp(health - value, 0, maxHealth);

        entityHPBar.ChangeHPByAmount(-value);

        if (health == 0)
        {
            Destroy(entityHPBar.gameObject);
            onHealthZeroed?.Invoke();
        }
    }
}
