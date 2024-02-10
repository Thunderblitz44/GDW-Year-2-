using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] internal Image filler;
    [SerializeField] internal TextMeshProUGUI text;
    [HideInInspector] public float maxHP;
    
    float hp;

    public void SetHPValue(int value)
    {
        if (!filler || !text) return;

        hp = Mathf.Clamp(value, 0, maxHP);
        SetHPInText();
        SetHPFill();
    }

    public virtual void ChangeHPByAmount(int amount)
    {
        if (!filler || !text) return;

        hp = Mathf.Clamp(hp + amount, 0, maxHP);
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
