using System;
using Unity.Netcode;
using UnityEngine;

public class HealthComponent : NetworkBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] float height = 1.5f;
    [SerializeField] GameObject hpBarPrefab;
    HPBar hpbar;
    EntityHPBar entityHPBar;

    NetworkVariable<float> health = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public Action onHealthZeroed;

    public override void OnNetworkSpawn()
    {
        if (IsOwner) health.Value = maxHealth;

        if (hpBarPrefab.GetComponent<EntityHPBar>())
        {
            entityHPBar = Instantiate(hpBarPrefab, GameSettings.instance.GetWorldCanvas()).GetComponent<EntityHPBar>();
            entityHPBar.transform.position = transform.position + Vector3.up * height;
            entityHPBar.maxHP = maxHealth;
            entityHPBar.SetHPValue(health.Value);
        }
        else if (IsOwner && hpBarPrefab.GetComponent<HPBar>())
        {
            hpbar = Instantiate(hpBarPrefab, GameSettings.instance.GetCanvas()).GetComponent<HPBar>();
            hpbar.maxHP = maxHealth;
            hpbar.SetHPValue(health.Value);
        }
    }

    private void Update()
    {
        if (!entityHPBar) return;

        entityHPBar.transform.position = transform.position + Vector3.up * height;
    }

    public void DeductHealth(float value)
    {
        if (IsOwner && hpbar) hpbar.ChangeHPByAmount(-value);
        else if (entityHPBar) entityHPBar.ChangeHPByAmount(-value);

        if (!IsOwner) return;
        if ((health.Value = GetHealth()) == 0) onHealthZeroed?.Invoke();
    }

    public float GetHealth() => hpbar == null ? entityHPBar == null ? 0 : entityHPBar.GetHP() : hpbar.GetHP();
}
