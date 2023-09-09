using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : NetworkBehaviour
{
    [SerializeField] Image filler;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject floatingTextPrefab;
    [HideInInspector] public float maxHP;
    public bool enableDamageNumbers = true;
    float hp;

    public void SetHPValue(float value)
    {
        if (!filler || !text) return;

        hp = Mathf.Clamp(value, 0, maxHP);
        SetHPInText();
        SetHPFill();
    }

    public void SetHPValue01(float value)
    {
        if (!filler || !text) return;

        value = Mathf.Clamp01(value);
        hp = maxHP * value;
        SetHPInText();
        SetHPFill();
    }

    public void ChangeHPByAmount(float amount)
    {
        if (!filler || !text) return;

        hp = Mathf.Clamp(hp + amount, 0, maxHP);
        SetHPInText();
        SetHPFill();
        TrySpawnHPChangeIndicator(amount);
    }

    public void ChangeHpByPercentage(float value01)
    {
        if (!filler || !text) return;

        value01 = Mathf.Clamp01(value01);
        hp = Mathf.Clamp(hp + value01 * maxHP, 0, maxHP);
        SetHPInText();
        SetHPFill();
        TrySpawnHPChangeIndicator(value01 * maxHP);
    }

    void SetHPInText()
    {
        text.text = $"HP : {hp} / {maxHP}";
    }

    void SetHPFill()
    {
        filler.fillAmount = maxHP > 0? hp / maxHP : 0;
    }

    void TrySpawnHPChangeIndicator(float changeValue)
    {
        if (!enableDamageNumbers) return;
        
        Transform t = Instantiate(floatingTextPrefab).transform;
        t.position = transform.position + Vector3.up * 1.5f;
        t.GetComponent<TextMeshProUGUI>().text = changeValue.ToString();
        t.GetComponent<NetworkObject>().Spawn(true);
        t.SetParent(GameSettings.instance.worldCanvas, true);
    }
}
