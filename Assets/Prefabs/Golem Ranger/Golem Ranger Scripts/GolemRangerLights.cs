using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemRangerLights : MonoBehaviour
{
    public Transform[] bonesToDraw; // Assign bones to this array in the inspector
    public Material lineMaterial; // Assign a material for the lines in the inspector

    private LineRenderer[] lineRenderers;

    void Start()
    {
        if (bonesToDraw == null || bonesToDraw.Length < 2 || lineMaterial == null)
        {
            Debug.LogWarning("Please assign bones and a material to draw lines.");
            return;
        }

        lineRenderers = new LineRenderer[bonesToDraw.Length - 1];

        for (int i = 0; i < lineRenderers.Length; i++)
        {
            lineRenderers[i] = CreateLineRenderer();
        }
    }

    void Update()
    {
        if (bonesToDraw == null || lineRenderers == null || bonesToDraw.Length < 2 || lineRenderers.Length != bonesToDraw.Length - 1)
            return;

        for (int i = 0; i < lineRenderers.Length; i++)
        {
            lineRenderers[i].SetPosition(0, bonesToDraw[i].position);
            lineRenderers[i].SetPosition(1, bonesToDraw[i + 1].position);
        }
    }

    LineRenderer CreateLineRenderer()
    {
        GameObject lineObj = new GameObject("Line");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        return lineRenderer;
    }
}
