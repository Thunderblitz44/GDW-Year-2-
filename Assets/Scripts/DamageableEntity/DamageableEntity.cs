using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DamageableEntity : MonoBehaviour, IDamageable
{
    [SerializeField] GameObject floatingTextPrefab;
    [SerializeField] bool enableDamageNumbers = true;
    [SerializeField] float damageNumberSpawnHeight = 1.5f;
    HealthComponent hp;

    void Start()
    {
        hp = GetComponent<HealthComponent>();
        hp.onHealthZeroed += OnHealthZeroed;
    }

    internal virtual void OnHealthZeroed()
    {
        Debug.Log(name + " died");
    }

    public void ApplyDamage(float damage, DamageTypes type)
    {
        if (!enableDamageNumbers) return;
        string msg = $"<color=#{(type == DamageTypes.physical ? StaticUtilities.physicalDamageColor.ToHexString() : StaticUtilities.magicDamageColor.ToHexString())}>{damage}</color>";

        Transform t = Instantiate(floatingTextPrefab).transform;
        t.position = transform.position + Vector3.up * damageNumberSpawnHeight;
        t.GetComponent<TextMeshProUGUI>().text = msg;
        t.SetParent(GameManager.Instance.worldCanvas, true);
    }

    public void ApplyDamageOverTime(float dps, DamageTypes type, float duration)
    {
        StartCoroutine(DamageOverTimeRoutine(dps, type, duration));
    }

    IEnumerator DamageOverTimeRoutine(float dps, DamageTypes type, float duration)
    {
        for (float i = 0, n = 0; i < duration; i += Time.deltaTime, n += Time.deltaTime)
        {
            if (n > StaticUtilities.damageOverTimeInterval)
            {
                n = 0;
                ApplyDamage(dps * StaticUtilities.damageOverTimeInterval, type);
            }
            yield return null;
        }
    }
}
