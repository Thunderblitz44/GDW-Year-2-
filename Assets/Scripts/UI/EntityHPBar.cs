using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EntityHPBar : HPBar
{
    float backgroundAlpha;
    [SerializeField] Image border;
    [SerializeField] float fadeDelay = 10f;
    [SerializeField] float fadeSpeed = 5f;
    Coroutine currentFadeRoutine;


    private void Awake()
    {
        SetTransparent();
    }

    private void Update()
    {
        if (Camera.main == null) return;
        transform.rotation = Camera.main.transform.rotation;
    }

    public void ChangeHPByAmount(int amount, bool autoHide = true)
    {
        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        Appear();
        base.ChangeHPByAmount(amount);
        if (autoHide) Disappear();
    }

    public void Appear()
    {
        filler.color = new Color(filler.color.r, filler.color.g, filler.color.b, 1);
        border.color = new Color(border.color.r, border.color.g, border.color.b, 1);
        lerpingFiller.color = new Color(lerpingFiller.color.r, lerpingFiller.color.g, lerpingFiller.color.b, 1);
    }

    public void Disappear(bool immediate = false)
    {
        if (gameObject.activeSelf) currentFadeRoutine = StartCoroutine(FadeRoutine(immediate));
    }

    IEnumerator FadeRoutine(bool immediate = false)
    {
        if (!immediate) yield return new WaitForSeconds(fadeDelay);

        for (float t = 1, b = backgroundAlpha; filler.color.a > 0; t -= Time.deltaTime * fadeSpeed, b -= Time.deltaTime * fadeSpeed)
        {
            float a = Mathf.Clamp01(t);
            filler.color = new Color(filler.color.r, filler.color.g, filler.color.b, a);
            border.color = new Color(border.color.r, border.color.g, border.color.b, a);
            yield return null;
        }
        currentFadeRoutine = null;
    }

    void SetTransparent()
    {
        filler.color = new Color(filler.color.r, filler.color.g, filler.color.b, 0);
        border.color = new Color(border.color.r, border.color.g, border.color.b, 0);
          lerpingFiller.color = new Color(lerpingFiller.color.r, lerpingFiller.color.g, lerpingFiller.color.b, 0);
               
    }

    private void OnDisable()
    {
        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
    }
}
