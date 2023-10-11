using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticUtilities
{
    public static Vector3 GetCameraDir()
    {
        Vector3 dir = Vector3.zero;
        dir.x = Camera.main.transform.forward.x;
        dir.z = Camera.main.transform.forward.z;
        return dir;
    }

    public static Vector3 GetCameraLook()
    {
        return Camera.main.transform.forward;
    }

    public static Vector3 CalculateLaunchVelocity(Vector3 startpoint, Vector3 endpoint, float overshoot)
    {
        float gravity = Physics.gravity.y;
        float displacementY = Math.Abs(endpoint.y - startpoint.y);
        float h = displacementY + overshoot;

        Vector3 displacementXZ = new Vector3(endpoint.x - startpoint.x, 0f, endpoint.z - startpoint.z);
        Vector3 velocityY = Vector3.up * MathF.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / (MathF.Sqrt(-2 * h / gravity)
            + MathF.Sqrt(2 * (displacementY - h) / gravity));
        return velocityXZ + velocityY;
    }
}
