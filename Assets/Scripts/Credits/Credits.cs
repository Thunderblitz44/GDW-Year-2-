using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] Image gameTitle;
    bool imgFadeEnd;

    [SerializeField] AnimatedText thanksMsg;
    [SerializeField] AnimatedText skipMsg;
    [SerializeField] RectTransform credits;

    ActionMap actions;
    bool scroll;
    [SerializeField] float scrollSpeed = 1f;
    [SerializeField] float scrollDelay = 1f;

    [Serializable]
    class AnimatedText
    {
        public TextMeshProUGUI textMesh;
        public float fadeSpeed;

        [HideInInspector] public bool startedFade;
        [HideInInspector] public bool finishedFade;
    }

    private void Start()
    {
        actions = new ActionMap();

        actions.Locomotion.Jump.performed += ctx =>
        {
            //skip
            StopAllCoroutines();
            SceneManager.LoadScene(0);
        };

        StartCoroutine(FadeRoutine(gameTitle));
    }

    private void Update()
    {
        if (imgFadeEnd && !scroll)
        {
            scroll = true;
        }

        if (scroll)
        {
            scrollDelay -= Time.deltaTime;
            if (scrollDelay > 0) return;

            if (!skipMsg.startedFade)
            {
                StartCoroutine(FadeRoutine(skipMsg));
                actions.Locomotion.Jump.Enable();
            }

            credits.position += Time.deltaTime * scrollSpeed * Vector3.up;
            gameTitle.transform.position += Time.deltaTime * scrollSpeed * Vector3.up;
        }

        if (credits.localPosition.y > 1050 && !thanksMsg.startedFade)
        {
            StartCoroutine(FadeRoutine(thanksMsg));
        }
    }

    private void OnDestroy()
    {
        actions.Dispose();
    }

    IEnumerator FadeRoutine(AnimatedText obj)
    {
        obj.startedFade = true;
        AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        for (float t = 0; t < 1; t += Time.deltaTime * obj.fadeSpeed)
        {
            obj.textMesh.color = Color.Lerp(Color.clear, Color.white, curve.Evaluate(t));
            yield return null;
        }

        obj.textMesh.color = Color.white;
        obj.finishedFade = true;
    }

    IEnumerator FadeRoutine(Image obj)
    {
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            obj.color = Color.Lerp(Color.clear, Color.white, curve.Evaluate(t));
            yield return null;
        }

        obj.color = Color.white;
        imgFadeEnd = true;
    }
}
