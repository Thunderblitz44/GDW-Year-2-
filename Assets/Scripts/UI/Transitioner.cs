using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Transitioner : MonoBehaviour
{
    public Action onFadedToBlack;
    public Action onFadedToClear;
    public float fadeSpeed = 1;
    [SerializeField] AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0,0,1,1);
    Image image;
    Coroutine currentRoutine;

    private void Awake()
    {
        if (!image) image = GetComponent<Image>();
    }

    public void FadeToBlack(float delay = 0)
    {
        if (currentRoutine != null) { StopCoroutine(currentRoutine); }
        currentRoutine = StartCoroutine(FadeToRoutine(delay, Color.black));
    }

    public void FadeToClear(float delay = 0)
    {
        if (currentRoutine != null) { StopCoroutine(currentRoutine); }
        currentRoutine = StartCoroutine(FadeToRoutine(delay, Color.clear));
    }

    IEnumerator FadeToRoutine(float delay, Color end)
    {
        if (!image) image = GetComponent<Image>();
        Color start = image.color;

        yield return new WaitForSeconds(delay);
        for (float t = 0; image.color != end; t += Time.deltaTime * fadeSpeed)
        {
            yield return null;
            image.color = Color.Lerp(start, end, fadeCurve.Evaluate(t));
        }
        currentRoutine = null;
        if (end.a > 0) onFadedToBlack?.Invoke();
        else onFadedToClear?.Invoke();
    }

    public void SetToBlack()
    {
        if (!image) image = GetComponent<Image>();
        image.color = Color.black;
    }

    public void SetToClear()
    {
        if (!image) image = GetComponent<Image>();
        image.color = Color.clear;
    }
}
