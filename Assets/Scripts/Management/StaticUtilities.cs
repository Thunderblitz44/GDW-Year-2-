using System.Collections.Generic;
using UnityEngine;

public static class StaticUtilities
{
    public static readonly float defaultFOV = 50f;
    public static readonly float damageOverTimeInterval = 0.5f;
    public static readonly Vector2 centerOfScreen = new(Screen.width / 2, Screen.height / 2);
    public static readonly float encounterStartDelay = 1f;
    public static readonly int groundLayer = 64;
    public static int visibleTargets;
    public static readonly string CURRENT_LEVEL = "level";
    public static readonly string CURRENT_CHECKPOINT = "checkpoint";
    public static readonly string LAST_ENCOUNTER = "encounter";
    public static readonly string CURRENT_PLAYER_HEALTH = "playerHealth";

    /*
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
    */

    public static Vector3 GetCameraDir()
    {
        return HorizontalizeVector(Camera.main.transform.forward);
    }

    public static float FastDistance(Vector3 first, Vector3 second)
    {
        return (first - second).sqrMagnitude;
    }
    public static Vector3 FlatDirection(Vector3 first, Vector3 second, float yOffset = 0f)
    {
        return first - BuildVector(second.x, first.y, second.z) + Vector3.up * yOffset;
    }

    public static Vector3 HorizontalizeVector(Vector3 vec)
    {
        return Vector3.right * vec.x + Vector3.forward * vec.z;
    }

    public static Vector3 BuildVector(float x, float y, float z)
    {
        return Vector3.right * x + Vector3.up * y + Vector3.forward * z;
    }

    public static void ShootProjectile(List<GameObject> projectilePool, Vector3 origin, Vector3 force)
    {
        foreach (var projectile in projectilePool)
        {
            if (projectile.activeSelf) continue;

            projectile.SetActive(true);
            projectile.transform.position = origin;
            projectile.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            break;
        }
    }

    public static bool TryToDamage(GameObject other, float damage)
    {
        IDamageable d;
        if (other.TryGetComponent(out d))
        {
            d.ApplyDamage(damage);
            return true;
        }
        return false;
    }

    public static bool TryToDamageOverTime(GameObject other, float damage, float duration)
    {
        IDamageable d;
        if (other.TryGetComponent(out d))
        {
            d.ApplyDamageOverTime(damage, duration);
            return true;
        }
        return false;
    }

}

