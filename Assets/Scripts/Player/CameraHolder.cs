using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    public Transform cameraHolder;

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraHolder.position;
    }
}
