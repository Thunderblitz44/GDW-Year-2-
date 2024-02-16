using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] protected Image filler;
    [HideInInspector] public float maxHP;
    
    float hp;

    public void SetHPValue(int value)
    {
        hp = Mathf.Clamp(value, 0, maxHP);
        SetHPInText();
        SetHPFill();
    }

    public virtual void ChangeHPByAmount(int amount)
    {
        hp = Mathf.Clamp(hp + amount, 0, maxHP);
        SetHPInText();
        SetHPFill();
    }

    void SetHPInText()
    {
        if (text) text.text = $"HP : {hp} / {maxHP}";
    }

    void SetHPFill()
    {
        if (filler) filler.fillAmount = maxHP > 0? hp / maxHP : 0;
    }
}
