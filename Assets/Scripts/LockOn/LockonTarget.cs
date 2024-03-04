using UnityEngine;

public class LockonTarget : MonoBehaviour
{
    bool startCheckVisibility = false;
    float visibilityCheckDelay = 0.2f;
    float timer = 0f;

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
        SlowUpdate();
    }

    void SlowUpdate()
    {
        Vector3 point = Camera.main.WorldToScreenPoint(transform.position);
        point.z = 0;

        // check if within screen bounds
        if (point.y < 0 || point.y > Screen.height || point.x < 0 || point.x > Screen.width)
        {
            // not visible
            SetInvisible();
            return;
        }

        // visible
        if (StaticUtilities.renderedEnemies.Contains(transform)) return;

        //Debug.Log(name + " is visible");
        StaticUtilities.renderedEnemies.Add(transform);
    }

    void SetInvisible()
    {
        // not visible
        if (!StaticUtilities.renderedEnemies.Contains(transform)) return;

        //Debug.Log(name + " is not visible");
        StaticUtilities.renderedEnemies.Remove(transform);
    }
}
