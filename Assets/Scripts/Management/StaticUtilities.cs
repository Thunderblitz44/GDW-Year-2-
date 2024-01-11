using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public static class StaticUtilities
{
    public const float defaultFOV = 50f;
    public const float damageOverTimeInterval = 0.25f;
    public static readonly Color physicalDamageColor = Color.white;
    public static readonly Color magicDamageColor = Color.magenta;
    public static readonly Vector2 centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
    public static int visibleTargets;


    // RANGED GOLEM ANIMATION KEYWORDS
    public const string GOLEM_RANGER_ATTACK = "Golem Ranger Shooting";


    public static List<Transform> SortByDistanceToScreenCenter(List<Transform> objects)
    {
        return objects.OrderBy(x => Vector2.Distance(centerOfScreen, (Vector2)Camera.main.WorldToScreenPoint(x.transform.position))).ToList();
    }

    public static List<Transform> SortByVisible(List<Transform> objects, int checklayer)
    {
        visibleTargets = 0;
        return objects.OrderBy(x => IsVisible(x, checklayer)).ToList();
    }

    public static bool IsVisible(Transform obj, int layer)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, obj.position - Camera.main.transform.position, out hit))
        {
            if (hit.transform.gameObject.layer == layer)
            {
                visibleTargets++; 
                return true;
            }
        }
        return false;
    }

    public static Vector3 GetCameraDir()
    {
        return Vector3.right * Camera.main.transform.forward.x +
            Vector3.forward * Camera.main.transform.forward.z;
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
