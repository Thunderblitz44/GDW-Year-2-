using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StaticUtilities
{
    public static readonly int groundLayer = 64;
    public static readonly float defaultFOV = 80f;
    public static readonly float damageOverTimeInterval = 0.5f;
    public static readonly float encounterStartDelay = 1f;
    public static readonly string CURRENT_LEVEL = "level";
    public static readonly string CURRENT_CHECKPOINT = "checkpoint";
    public static readonly string LAST_ENCOUNTER = "encounter";
    public static readonly string CURRENT_PLAYER_HEALTH = "playerHealth";
    public static readonly AnimationCurve easeCurve01 = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public static readonly List<Transform> renderedEnemies = new();
    public static Plane[] cameraFrustrumPlanes;


    public static List<Transform> SortByDistanceToScreenCenter(List<Transform> objects, float cutoffRadius)
    {
        return objects.Where(t => Vector2.Distance(GetCenterOfScreen(), (Vector2)Camera.main.WorldToScreenPoint(t.position)) < cutoffRadius)
            .OrderBy(x => Vector2.Distance(GetCenterOfScreen(), (Vector2)Camera.main.WorldToScreenPoint(x.position))).ToList();
    }

    public static List<Transform> SortByVisible(List<Transform> objects, float range, LayerMask blockingLayers)
    {
        return objects.Where(t => IsVisible(t, range, blockingLayers)).ToList();
    }

    public static bool IsVisible(Transform obj, float range, LayerMask blockingLayers)
    {
        if (Physics.Raycast(Camera.main.transform.position, obj.position - Camera.main.transform.position, out RaycastHit hit, range, blockingLayers, QueryTriggerInteraction.Ignore))
        {
            if (obj == hit.collider.transform)
            {
                return true;
            }
        }
        return false;
    }
    /*public static Vector3 CalculateLaunchVelocity(Vector3 startpoint, Vector3 endpoint, float overshoot)
    {
        float gravity = Physics.gravity.y;
        float displacementY = Mathf.Abs(endpoint.y - startpoint.y);
        float h = displacementY + overshoot;

        Vector3 displacementXZ = new Vector3(endpoint.x - startpoint.x, 0f, endpoint.z - startpoint.z);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * h / gravity)
            + Mathf.Sqrt(2 * (displacementY - h) / gravity));
        return velocityXZ + velocityY;
    }*/


    public static Vector2 GetCenterOfScreen()
    {
        return Vector2.right * Screen.width / 2 + Vector2.up * Screen.height / 2;
    }

    public static Vector3 GetCameraDir()
    {
        return HorizontalizeVector(Camera.main.transform.forward * 2).normalized;
    }

    public static float FastDistance(Vector3 first, Vector3 second)
    {
        return (first - second).sqrMagnitude;
    }

    /// <summary>
    /// Returns a flat direction based on first's Y value
    /// </summary>
    /// <param name="to">target</param>
    /// <param name="from">start</param>
    /// <param name="yOffset"></param>
    /// <returns></returns>
    public static Vector3 FlatDirection(Vector3 to, Vector3 from, float yOffset = 0f)
    {
        return ((to - BuildVector(from.x, to.y, from.z) + Vector3.up * yOffset) * 2).normalized;
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

    public static bool TryToDamage(GameObject other, int damage)
    {
        IDamageable d;
        if (other.TryGetComponent(out d))
        {
            d.ApplyDamage(damage);
            return true;
        }
        return false;
    }

    public static bool TryToDamageOverTime(GameObject other, int damage, float duration)
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

