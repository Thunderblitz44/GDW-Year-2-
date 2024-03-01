using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkCubeAttack : MonoBehaviour
{
    [SerializeField] private GameObject waterFx;
    [SerializeField] private BoxCollider boxCollider;

    private bool playerInside = false;
    private FogMode targetFogMode;
    private float targetFogStartDistance;
    private float targetFogEndDistance;
    private Color targetFogColor;
    private float fogTransitionSpeed = 0.5f; // Adjust transition speed as needed

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            SetFogSettings(FogMode.Linear, 1.0f, 50.0f, new Color(50.0f / 255.0f, 20.0f / 255.0f, 128.0f / 255.0f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            SetFogSettings(FogMode.ExponentialSquared, 0.0015f, 0.0f, new Color(185.0f / 255.0f, 92.0f / 255.0f, 128.0f / 255.0f));
        }
    }

    private void Update()
    {
        if (playerInside)
        {
            // Gradually transition to target fog settings
            RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, targetFogStartDistance, Time.deltaTime * fogTransitionSpeed);
            RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, targetFogEndDistance, Time.deltaTime * fogTransitionSpeed);
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetFogColor, Time.deltaTime * fogTransitionSpeed);
        }
    }

    private void SetFogSettings(FogMode mode, float startDistance, float endDistance, Color color)
    {
        targetFogMode = mode;
        targetFogStartDistance = startDistance;
        targetFogEndDistance = endDistance;
        targetFogColor = color;
        RenderSettings.fogMode = mode; // Assign target fog mode directly
    }
}

