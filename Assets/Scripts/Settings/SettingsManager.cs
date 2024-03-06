using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [SerializeField] GameSettings defaultSettings;
    public GameSettings DefaultSettings { get { return defaultSettings; } }

    [SerializeField] GameSettings settings;
    public GameSettings Settings { get { return settings; } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("There were multiple SettingsManager scripts!");
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
