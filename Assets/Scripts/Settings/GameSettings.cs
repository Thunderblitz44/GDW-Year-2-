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

    /*
Control Settings
Keyboard & Mouse Control Layout
Controller Control Layout
Control Rebinding?
Reset to default Controls
Inverted Controls
Reset to default Control Settings in general

Can you swap between controller and m&k on the fly?
Extended m&k controls (Mouse button 3&4 for example)

Video Settings
Different resolution options
Different screen ratios
Fullscreen, windowed, and borderless
Reset to default Video Settings

Graphic Settings
Brightness Modification
Motion Blur on-off
Individual Graphic Quality Levels
	Texture quality, Shadow quality, Reflection quality, Particle effect quality (visual effects, explosions, atmospheric effects, volumetric clouds, fog, water), Anti-Aliasing, V-Sync, FPS Limit, Field of view, Ambient Occlusion, Depth of Field
Reset to default Graphic Settings


Tutorial
This tab will show multiple different buttons each named after their respective mechanic and when selected there will be a gif and a short explanation of the mechanic.

Miscellaneous
If a player tries to leave the options without saving their settings they will be prompted with a “Do you want to save your settings” option.

Reset to default settings overall

Language change setting (I could get the necessary translations)

     */
}
