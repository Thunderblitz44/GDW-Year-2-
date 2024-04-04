using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider ambientSlider;
    [SerializeField] Slider sensSlider;
    [SerializeField] Toggle aimAssistToggle;

    [SerializeField] GameSettings activeSettings;
    [SerializeField] GameSettings defaultSettings;

    bool changed;

    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        ambientSlider.onValueChanged.AddListener(OnAmbientChanged);
        sensSlider.onValueChanged.AddListener(OnSensitivityChanged);
        aimAssistToggle.onValueChanged.AddListener(OnAutoAssistChanged);
        
        // load settings
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume", defaultSettings.masterVolume);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", defaultSettings.SFXVolume);
        ambientSlider.value = PlayerPrefs.GetFloat("ambianceVolume", defaultSettings.ambianceVolume);
        sensSlider.value = PlayerPrefs.GetFloat("sensitivity", defaultSettings.sensitivity);
        aimAssistToggle.isOn = PlayerPrefs.GetInt("autoLock", defaultSettings.autoLock ? 1 : 0) == 1 ? true : false;
        Save();
        gameObject.SetActive(false);
    }

    public void OnAutoAssistChanged(bool toggle)
    {
        activeSettings.autoLock = toggle;
        PlayerPrefs.SetInt("autoLock", toggle ? 1 : 0);
        Save();
    }

    public void OnDebugPanelChanged(bool toggle)
    {
        activeSettings.debugConsole = toggle;
        PlayerPrefs.SetInt("debugConsole", toggle ? 1 : 0);
        Save();
    }

    public void OnSensitivityChanged(float value)
    {
        activeSettings.sensitivity = value;
        PlayerPrefs.SetFloat("sensitivity", value);
        Save();
    }

    public void OnAmbientChanged(float value)
    {
        activeSettings.ambianceVolume = value;
        PlayerPrefs.SetFloat("ambianceVolume", value);
        Save();
    }

    public void OnSfxChanged(float value)
    {
        activeSettings.SFXVolume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
        Save();
    }

    public void OnMasterChanged(float value)
    {
        activeSettings.masterVolume = value;
        PlayerPrefs.SetFloat("masterVolume", value);
        Save();
    }

    public void ResetSettings()
    {
        masterSlider.value = defaultSettings.masterVolume;
        sfxSlider.value = defaultSettings.SFXVolume;
        ambientSlider.value = defaultSettings.ambianceVolume;
        sensSlider.value = defaultSettings.sensitivity;
        aimAssistToggle.isOn = defaultSettings.autoLock;
        Save();
    }

    public void Save()
    {
        PlayerPrefs.Save();
        if (LevelManager.Instance) LevelManager.Instance.PlayerScript.SettingsChanged();
    }
}
