using UnityEngine;

public class EntityHPBar : HPBar
{
    private void Update()
    {
        if (Camera.main == null) return;
        transform.rotation = Camera.main.transform.rotation;
    }
}
