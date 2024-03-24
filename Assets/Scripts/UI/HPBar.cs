using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] protected Image filler;
    [SerializeField] protected Image lerpingFiller;
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
        if (filler && lerpingFiller)
        {
            float fillAmount = maxHP > 0 ? hp / maxHP : 0;

          
            filler.fillAmount = fillAmount;

            
            StartCoroutine(LerpFillAmount(lerpingFiller, fillAmount));
        }
    }

    IEnumerator LerpFillAmount(Image lerpingFiller, float targetFill)
    {
        float startTime = Time.time;
        float startFill = lerpingFiller.fillAmount;
        float duration = 0.5f; 

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            lerpingFiller.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        lerpingFiller.fillAmount = targetFill;
    }
}
