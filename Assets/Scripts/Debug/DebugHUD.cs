using TMPro;
using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    public static DebugHUD instance;

    [SerializeField] TextMeshProUGUI speedTxt;
    [SerializeField] TextMeshProUGUI timerTxt;
    [SerializeField] Transform controlsPanel;
    [SerializeField] GameObject controlTxtPrefab;
    [SerializeField] bool mouseKeyboardControls = true;
    bool oldmouseKeyboardControls;

    ActionMap playerActions;

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
        oldmouseKeyboardControls = mouseKeyboardControls;

    }

    void Update()
    {
        if (mouseKeyboardControls != oldmouseKeyboardControls)
        {
            oldmouseKeyboardControls = mouseKeyboardControls;
            DisplayControls(playerActions, mouseKeyboardControls);
        }
    }

    public void SetSpeed(float value)
    {
        if (!speedTxt) return;
        speedTxt.text = $"Speed : {value.ToString("0.00")}u/s";
    }

    public void SetDebugText(string value)
    {
        if (!timerTxt) return;
        timerTxt.text = value;
    }

    public void DisplayControls(ActionMap controls, bool mouseKeyboard = true)
    {
        if (playerActions == null) playerActions = controls;

        // delete old controls if there are any
        if (controlsPanel.childCount > 1)
        {
            for (int i = 1; i < controlsPanel.childCount; i++)
            {
                Destroy(controlsPanel.GetChild(i).gameObject);
            }
        }

        foreach (var map in controls.asset.actionMaps)
        {
            if (map.name == "Menus" || map.name == "CameraControl") continue;
            foreach (var action in map.actions)
            {
                TextMeshProUGUI t = Instantiate(controlTxtPrefab, controlsPanel).GetComponent<TextMeshProUGUI>();
                t.text = string.Concat(action.name, ": ");
                int b = 0;
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (mouseKeyboard && !action.bindings[i].path.Contains("<Keyboard>") &&
                        !action.bindings[i].path.Contains("<Mouse>")) continue;
                    else if (!mouseKeyboard && !action.bindings[i].path.Contains("<Gamepad>")) continue;
                    
                    if (b++ > 0) t.text += ", ";
                    t.text += action.bindings[i].ToDisplayString();
                }
            }
        }
    }
}
