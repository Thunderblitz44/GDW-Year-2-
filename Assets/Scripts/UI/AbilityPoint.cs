using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPoint : MonoBehaviour
{
    Image img;
    public Action onAbilityPointCharged;
    public bool isSpent { get; private set; }

    private void Awake()
    {
        img = transform.GetChild(1).GetComponent<Image>();
    }

    public void SetSpent()
    {
        img.fillAmount = 0f;
        isSpent = true;
    }

    public IEnumerator Recharge(float time)
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            img.fillAmount = t / time;
            yield return null;
        }
        img.fillAmount = 1f;

        isSpent = false;
        onAbilityPointCharged?.Invoke();
    }
}
