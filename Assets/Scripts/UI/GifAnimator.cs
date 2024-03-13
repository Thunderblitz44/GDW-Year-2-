using UnityEngine;
using UnityEngine.UI;

public class GifAnimator : MonoBehaviour
{
    [SerializeField] Sprite[] frames;
    Image gif;

    int index;
    float delay = 0.0625f;
    float timer;

    private void Awake()
    {
        gif = GetComponent<Image>();
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer >= delay)
        {
            timer = 0;
            index = index + 1 < frames.Length ? index + 1 : 0;
            gif.sprite = frames[index];
        }
    }
}
