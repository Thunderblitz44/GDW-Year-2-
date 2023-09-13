using Unity.Netcode;
using UnityEngine;

public enum DamageTypes
{
    physical,
    magic
}

public class GameSettings : NetworkBehaviour
{
    public static GameSettings instance;

    public float damageOverTimeInterval = 0.25f;
    public float defaultFOV = 50f;
    [SerializeField] Transform canvas;
    [SerializeField] Transform worldCanvas;

    public Color physicalDamageColor = Color.white;
    public Color magicDamageColor = Color.magenta;

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
    public Transform GetWorldCanvas() => worldCanvas;
}
