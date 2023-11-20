using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DamageableEntity : NetworkBehaviour, IDamageable
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
        ApplyDamageServerRpc(damage, type);
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


    [ClientRpc]
    void SpawnFloatingTextClientRpc(string message)
    {
        if (!IsClient) return;

        Transform t = Instantiate(floatingTextPrefab).transform;
        t.position = transform.position + Vector3.up * damageNumberSpawnHeight;
        t.GetComponent<TextMeshProUGUI>().text = message;
        t.SetParent(GameManager.instance.GetWorldCanvas(), true);
    }

    [ServerRpc(RequireOwnership = false)]
    void ApplyDamageServerRpc(float damage, DamageTypes type)
    {
        if (!IsServer) return;

        ApplyDamageClientRpc(damage);

        if (!enableDamageNumbers) return;
        string msg = $"<color=#{(type == DamageTypes.physical ? StaticUtilities.physicalDamageColor.ToHexString() : StaticUtilities.magicDamageColor.ToHexString())}>{damage}</color>";
        SpawnFloatingTextClientRpc(msg);
    }

    [ClientRpc]
    void ApplyDamageClientRpc(float damage)
    {
        hp.DeductHealth(damage);
    }
}
