using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DamageableEntity : MonoBehaviour, IDamageable
{
    HealthComponent hp;
    [SerializeField] float damageNumberSpawnHeight = 1.5f;
    [SerializeField] GameObject floatingTextPrefab;
    [SerializeField] bool enableDamageNumbers = true;

    void Awake()
    {
        hp = GetComponent<HealthComponent>();
        if (hp) hp.onHealthZeroed += OnHealthZeroed;
    }

    internal virtual void OnHealthZeroed()
    {

    }

    public void ApplyDamage(float damage, DamageTypes type)
    {
        hp.DeductHealth(damage);
        if (!enableDamageNumbers) return;

        string msg = $"<color=#{(type == DamageTypes.physical? GameSettings.instance.physicalDamageColor.ToHexString() : GameSettings.instance.magicDamageColor.ToHexString())}>{damage}</color>";
        SpawnFloatingText(msg);
    }

    public void ApplyDamageOverTime(float dps, DamageTypes type, float duration)
    {
        StartCoroutine(DamageOverTimeRoutine(dps, type, duration));
    }

    IEnumerator DamageOverTimeRoutine(float dps, DamageTypes type, float duration)
    {
        for (float i = 0, n = 0; i < duration; i += Time.deltaTime, n += Time.deltaTime)
        {
            if (n > GameSettings.instance.damageOverTimeInterval)
            {
                n = 0;
                ApplyDamage(dps * GameSettings.instance.damageOverTimeInterval, type);
            }
            yield return null;
        }
    }

    void SpawnFloatingText(string message)
    {
        Transform t = Instantiate(floatingTextPrefab).transform;
        t.position = transform.position + Vector3.up * damageNumberSpawnHeight;
        t.GetComponent<TextMeshProUGUI>().text = message;
        t.GetComponent<NetworkObject>().Spawn(true);
        t.SetParent(GameSettings.instance.GetWorldCanvas(), true);
    }
}
