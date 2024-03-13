using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    [SerializeField] float offsetMultiplier = 1f;
    [SerializeField] float smoothTime = .3f;
    Vector3 startPos;
    Vector3 velocity;

    private void Awake()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        Vector3 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        transform.position = Vector3.SmoothDamp(transform.position, startPos + (offset * offsetMultiplier), ref velocity, smoothTime);
    }
}
