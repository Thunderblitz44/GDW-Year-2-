using UnityEngine;

[CreateAssetMenu(fileName ="Settings", menuName ="Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Gameplay")]
    public bool autoLock;
    public bool debugConsole;
    [Range(1, 100)]public float sensitivity = 15; 
    public float MouseSensXForCinemachine { get { return sensitivity / 100f; } }
    public float MouseSensYForCinemachine { get { return sensitivity / 10000f; } }
    public float GamepadSensXForCinemachine { get { return sensitivity / 10f; } }
    public float GamepadSensYForCinemachine { get { return sensitivity / 1000f; } }

    [Header("Audio")]
    [Range(0f, 1)] public float masterVolume;
    [Range(0f, 1f)] public float SFXVolume;
    [Range(0f, 1f)] public float ambianceVolume;
}
