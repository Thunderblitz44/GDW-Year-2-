using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public static class StaticUtilities
{
    public static float defaultFOV = 50f;
    public static float damageOverTimeInterval = 0.25f;
    public static Color physicalDamageColor = Color.white;
    public static Color magicDamageColor = Color.magenta;
    public static Vector2 centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);

    public static List<Transform> SortByDistanceToScreenCenter(List<Transform> objects)
    {
        return objects.OrderBy(x => Vector2.Distance(centerOfScreen, (Vector2)Camera.main.WorldToScreenPoint(x.transform.position))).ToList();
    }

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
        float displacementY = Mathf.Abs(endpoint.y - startpoint.y);
        float h = displacementY + overshoot;

        Vector3 displacementXZ = new Vector3(endpoint.x - startpoint.x, 0f, endpoint.z - startpoint.z);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * h / gravity)
            + Mathf.Sqrt(2 * (displacementY - h) / gravity));
        return velocityXZ + velocityY;
    }
}
