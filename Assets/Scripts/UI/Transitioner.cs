using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transitioner : MonoBehaviour
{
    public Action onFadedToBlack;
    public float fadeSpeed = 1;
    [SerializeField] AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0,0,1,1);
    Image image;
    Coroutine currentRoutine;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void FadeToBlack(float delay = 0)
    {
        if (currentRoutine != null) { StopCoroutine(currentRoutine); }
        currentRoutine = StartCoroutine(FadeToBlackRoutine(delay));
    }

    IEnumerator FadeToBlackRoutine(float delay)
    {
        Color start = image.color;

        yield return new WaitForSeconds(delay);
        for (float t = 0; image.color != Color.black; t += Time.deltaTime * fadeSpeed) 
        {
            yield return null;
            image.color = Color.Lerp(start, Color.black, fadeCurve.Evaluate(t));
        }
        currentRoutine = null;
        onFadedToBlack?.Invoke();
    }

    public void FadeToClear(float delay = 0)
    {
        if (currentRoutine != null) { StopCoroutine(currentRoutine); }
        currentRoutine = StartCoroutine(FadeToClearRoutine(delay));
    }

    IEnumerator FadeToClearRoutine(float delay)
    {
        Color start = image.color;

        yield return new WaitForSeconds(delay);
        for (float t = 0; image.color != Color.clear; t += Time.deltaTime * fadeSpeed)
        {
            yield return null;
            image.color = Color.Lerp(start, Color.clear, fadeCurve.Evaluate(t));
        }
        currentRoutine = null;
    }

    public void SetToBlack()
    {
        image.color = Color.black;
    }

    public void SetToClear()
    {
        image.color = Color.clear;
    }
}
