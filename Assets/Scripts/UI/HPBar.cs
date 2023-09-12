using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : NetworkBehaviour
{
    [SerializeField] internal Image filler;
    [SerializeField] internal TextMeshProUGUI text;
    [HideInInspector] public float maxHP;
    
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

    public virtual void ChangeHPByAmount(float amount)
    {
        if (!filler || !text) return;

        hp = Mathf.Clamp(hp + amount, 0, maxHP);
        SetHPInText();
        SetHPFill();
    }

    public virtual void ChangeHpByPercentage(float value01)
    {
        if (!filler || !text) return;

        value01 = Mathf.Clamp01(value01);
        hp = Mathf.Clamp(hp + value01 * maxHP, 0, maxHP);
        SetHPInText();
        SetHPFill();
    }

    void SetHPInText()
    {
        text.text = $"HP : {hp} / {maxHP}";
    }

    void SetHPFill()
    {
        filler.fillAmount = maxHP > 0? hp / maxHP : 0;
    }

    
}
