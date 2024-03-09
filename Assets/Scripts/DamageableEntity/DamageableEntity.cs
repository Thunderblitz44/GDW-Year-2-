using System.Collections;
using TMPro;
using UnityEngine;

public class DamageableEntity : MonoBehaviour, IDamageable
{
    public bool enableDamageNumbers = true;
    [SerializeField] float damageNumberSpawnHeight = 1.5f;
    public bool isInvincible;
    protected HealthComponent hp;
    protected Animator animator2;
    protected virtual void Awake()
    {
        hp = GetComponent<HealthComponent>();
        animator2 = GetComponent<Animator>();
        if (hp) hp.onHealthZeroed += OnHealthZeroed;
    }

    protected virtual void OnHealthZeroed()
    {
        if (animator2)
        {
            animator2.SetTrigger("Die");
            Destroy(hp);
        }
        else Destroy(gameObject);
    }
   
    public virtual void ApplyDamage(int damage)
    {
        if (!hp) return;
        if (isInvincible) damage = 0;
        hp.DeductHealth(damage);
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Elana Hit Success", gameObject);

        if (!enableDamageNumbers) return;

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
}
