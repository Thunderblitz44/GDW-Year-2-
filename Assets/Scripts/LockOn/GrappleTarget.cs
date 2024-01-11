using UnityEngine;

public class GrappleTarget : MonoBehaviour
{
    bool startCheckVisibility = false;
    float visibilityCheckDelay = 0.1f;
    float timer = 0f;

    [SerializeField] int priority = 0;

    private void OnBecameVisible()
    {
        startCheckVisibility = true;
    }

    private void OnBecameInvisible()
    {
        startCheckVisibility = false;
        
        SetInvisible();
    }

    private void Update()
    {
        if (!startCheckVisibility) return;

        timer += Time.deltaTime;
        if (timer < visibilityCheckDelay) return;
        timer = 0f;

        Vector3 point;

        if (Camera.main != null)
        {
            point = Camera.main.WorldToScreenPoint(transform.position);
        }
        else
        {
            point = GameManager.Instance.lobbyCamera.WorldToScreenPoint(transform.position);
        }
        point.z = 0;

        // check if within screen bounds
        if (point.y < 0 || point.y > Screen.height || point.x < 0 || point.x > Screen.width)
        {
            // not visible
            SetInvisible();
            return;
        }

        // visible
        if (GameManager.Instance.renderedGrappleTargets.Contains(transform)) return;

        //Debug.Log(name + " is visible");
        GameManager.Instance.renderedGrappleTargets.Add(transform);
    }


    void SetInvisible()
    {
        // not visible
        if (!GameManager.Instance.renderedGrappleTargets.Contains(transform)) return;

        //Debug.Log(name + " is not visible");
        GameManager.Instance.renderedGrappleTargets.Remove(transform);
    }

    public int GetPriorityLevel() => priority;
}
