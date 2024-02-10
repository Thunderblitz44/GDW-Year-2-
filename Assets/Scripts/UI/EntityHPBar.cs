using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EntityHPBar : HPBar
{
    float backgroundAlpha;
    [SerializeField] float fadeDelay = 10f;
    [SerializeField] float fadeSpeed = 5f;
    [SerializeField] Image background;
    [SerializeField] Image border;
    Coroutine currentFadeRoutine;

    private void Awake()
    {
        backgroundAlpha = background.color.a;
        SetTransparent();
    }

    private void Update()
    {
        if (Camera.main == null) return;
        transform.rotation = Camera.main.transform.rotation;
    }

    public override void ChangeHPByAmount(int amount)
    {
        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        Appear();
        base.ChangeHPByAmount(amount);
        Disappear();
    }

    void Appear()
    {
        filler.color = new Color(filler.color.r, filler.color.g, filler.color.b, 1);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        background.color = new Color(background.color.r, background.color.g, background.color.b, backgroundAlpha);
        border.color = new Color(border.color.r, border.color.g, border.color.b, 1);
    }

    void Disappear()
    {
        currentFadeRoutine = StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(fadeDelay);

        for (float t = 1, b = backgroundAlpha; filler.color.a > 0; t -= Time.deltaTime * fadeSpeed, b -= Time.deltaTime * fadeSpeed)
        {
            float a = Mathf.Clamp01(t);
            filler.color = new Color(filler.color.r, filler.color.g, filler.color.b, a);
            text.color = new Color(text.color.r, text.color.g, text.color.b, a);
            background.color = new Color(background.color.r, background.color.g, background.color.b, Mathf.Clamp01(b));
            border.color = new Color(border.color.r, border.color.g, border.color.b, a);
            yield return null;
        }
    }

    void SetTransparent()
    {
        filler.color = new Color(filler.color.r, filler.color.g, filler.color.b, 0);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
        border.color = new Color(border.color.r, border.color.g, border.color.b, 0);
    }
}
