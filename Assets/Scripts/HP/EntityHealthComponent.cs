using UnityEngine;

public class EntityHealthComponent : HealthComponent
{
    [SerializeField] float height = 1.5f;
    [SerializeField] bool autoHide = true;
    EntityHPBar entityHPBar;

    public bool DestroyOnHPZero { get; set; } = true;

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

    private void OnDisable()
    {
        if (entityHPBar) entityHPBar.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (entityHPBar) entityHPBar.gameObject.SetActive(true);
    }

    public override void DeductHealth(int value)
    {
        health = Mathf.Clamp(health - value, 0, maxHealth);

        entityHPBar.ChangeHPByAmount(-value, autoHide);

        if (health == 0)
        {
            if (DestroyOnHPZero) Destroy(entityHPBar.gameObject);
            else entityHPBar.gameObject.SetActive(false);
            onHealthZeroed?.Invoke();
        }
    }

    public override void SetHealth(int value)
    {
        health = value;
        if (entityHPBar) entityHPBar.SetHPValue(health);
    }

    public void ShowHPBar() => entityHPBar.Appear();
    public void HideHPBar() => entityHPBar.Disappear(true);
}
