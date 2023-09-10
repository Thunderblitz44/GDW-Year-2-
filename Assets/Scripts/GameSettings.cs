using Unity.Netcode;
using UnityEngine;

public class GameSettings : NetworkBehaviour
{
    public static GameSettings instance;

    public float damageOverTimeInterval = 0.25f;
    public Transform canvas;
    public Transform worldCanvas;
    public DashUI dashUI;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public Transform GetCanvas() => canvas;
}
