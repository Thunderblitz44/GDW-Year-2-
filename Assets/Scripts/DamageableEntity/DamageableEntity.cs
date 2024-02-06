using System.Collections;
using TMPro;
using UnityEngine;

public class DamageableEntity : MonoBehaviour, IDamageable
{
    [SerializeField] GameObject floatingTextPrefab;
    [SerializeField] bool enableDamageNumbers = true;
    [SerializeField] float damageNumberSpawnHeight = 1.5f;
    public bool isInvincible;
    internal HealthComponent hp;

    internal virtual void Awake()
    {
        hp = GetComponent<HealthComponent>();
        if (hp) hp.onHealthZeroed += OnHealthZeroed;
    }

    internal virtual void OnHealthZeroed()
    {
        Destroy(gameObject);
    }

    public void ApplyDamage(float damage)
    {
        if (!hp || isInvincible) return;
        hp.DeductHealth(damage);

        if (!enableDamageNumbers) return;

        Transform t = Instantiate(floatingTextPrefab).transform;
        t.position = transform.position + Vector3.up * damageNumberSpawnHeight;
        t.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        t.SetParent(LevelManager.Instance.WorldCanvas, true);
    }

    public void ApplyDamageOverTime(float dps, float duration)
    {
        StartCoroutine(DamageOverTimeRoutine(dps, duration));
    }

    IEnumerator DamageOverTimeRoutine(float dps, float duration)
    {
        for (float i = 0, n = 0; i < duration; i += Time.deltaTime, n += Time.deltaTime)
        {
            if (n > StaticUtilities.damageOverTimeInterval)
            {
                n = 0;
                ApplyDamage(dps * StaticUtilities.damageOverTimeInterval);
            }
            yield return null;
        }
    }
}
