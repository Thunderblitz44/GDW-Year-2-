using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class FloatingText : NetworkBehaviour
{
    TextMeshProUGUI textmesh;
    [SerializeField] AnimationCurve fade;
    [SerializeField] AnimationCurve speed;

    private void Update()
    {
        if (Camera.main == null) return;
        transform.rotation = Camera.main.transform.rotation;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(FloatingTextRoutine());
    }

    private IEnumerator FloatingTextRoutine()
    {
        textmesh = GetComponent<TextMeshProUGUI>();
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f;

        // a = alpha, t = time
        for (float a = 1, t = 0; a > 0; a = fade.Evaluate(t), t += Time.deltaTime)
        {
            // lerp towards endPos, while fading out
            transform.position = Vector3.Lerp(startPos, endPos, speed.Evaluate(t));
            textmesh.color = new Color(textmesh.color.r, textmesh.color.g, textmesh.color.b, a);
            yield return null;
        }

        Destroy(gameObject);
    }
}
