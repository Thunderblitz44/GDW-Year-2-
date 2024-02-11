using System.Collections;
using TMPro;
using UnityEngine;

public class DamageableEntity : MonoBehaviour, IDamageable
{
    public bool enableDamageNumbers = true;
    [SerializeField] float damageNumberSpawnHeight = 1.5f;
    //GameObject floatingTextPrefab;
    public bool isInvincible;
    protected HealthComponent hp;

    protected virtual void Awake()
    {
        hp = GetComponent<HealthComponent>();
        if (hp) hp.onHealthZeroed += OnHealthZeroed;

        /*if (enableDamageNumbers)
        {
            Debug.LogWarningFormat("Assign floating text prefab for {0}", name);
            LoadFloatingTextPrefab();
        }*/
    }

    protected virtual void OnHealthZeroed()
    {
        Destroy(gameObject);
    }

    public virtual void ApplyDamage(int damage)
    {
        if (!hp) return;
        if (isInvincible) damage = 0;
        hp.DeductHealth(damage);

        if (!enableDamageNumbers) return;
        //else if (!floatingTextPrefab) LoadFloatingTextPrefab();

        Transform t = Instantiate(LevelManager.Instance.floatingTextPrefab).transform;
        t.position = transform.position + Vector3.up * damageNumberSpawnHeight;
        t.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        t.SetParent(LevelManager.Instance.WorldCanvas, true);
    }

    public virtual void ApplyDamageOverTime(int damage, float duration)
    {
        StartCoroutine(DamageOverTimeRoutine(damage, duration));
    }

    IEnumerator DamageOverTimeRoutine(int damage, float duration)
    {
        for (float i = 0, n = 0; i < duration; i += Time.deltaTime, n += Time.deltaTime)
        {
            if (n > StaticUtilities.damageOverTimeInterval)
            {
                n = 0;
                ApplyDamage(damage);
            }
            yield return null;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other);
        ApplyDamage(1);
    }

    /*void LoadFloatingTextPrefab()
    {
#if UNITY_EDITOR
        floatingTextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FloatingText.prefab");
        if (!floatingTextPrefab) Debug.LogWarning("Can't find FloatingTextPrefab in Assets/Prefabs/FloatingText.prefab");
#endif
    }*/
}
