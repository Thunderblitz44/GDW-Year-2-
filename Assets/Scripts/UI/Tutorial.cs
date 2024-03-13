using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI walk;
    [SerializeField] TextMeshProUGUI run;
    [SerializeField] TextMeshProUGUI jump;
    [SerializeField] TextMeshProUGUI dodge;
    [SerializeField] TextMeshProUGUI melee;
    [SerializeField] TextMeshProUGUI ranged;
    [SerializeField] TextMeshProUGUI fireTornado;
    [SerializeField] TextMeshProUGUI recall;

    ActionMap map;

    bool mk = true;
    bool oldmk = true;

    private void Awake()
    {
        map = new ActionMap();
        
        DisplayControls(map);

        map.General.DeviceTest.started += ctx =>
        {
            if (ctx.control.ToString().Contains("Stick")) mk = false;
            else mk = true;

            if (mk != oldmk)
            {
                DisplayControls(map, mk);
            }
            oldmk = mk;
        };

        map.General.Enable();
    }

    private void OnDestroy()
    {
        map.Dispose();
    }

    public void DisplayControls(ActionMap controls, bool mouseKeyboard = true)
    {
        foreach (var map in controls.asset.actionMaps)
        {
            if (map.name == "Menus" || map.name == "CameraControl") continue;
            foreach (var action in map.actions)
            {
                string t = "";
                int b = 0;
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (mouseKeyboard && !action.bindings[i].path.Contains("<Keyboard>") &&
                        !action.bindings[i].path.Contains("<Mouse>")) continue;
                    else if (!mouseKeyboard && !action.bindings[i].path.Contains("<Gamepad>")) continue;

                    if (b++ > 0) t += ", ";
                    t += action.bindings[i].ToDisplayString();
                }

                switch (action.name)
                {
                    case "Move":
                        walk.text = t;
                        break;
                    case "Jump":
                        jump.text = t;
                        break;
                    case "Run":
                        run.text = t;
                        break;
                    case "Dodge":
                        dodge.text = t;
                        break;
                    case "Portal":
                        recall.text = t ;
                        break;
                    case "FireTornado":
                        fireTornado.text = t;
                        break;
                    case "PrimaryAttack":
                        melee.text = t;
                        break;
                    case "SecondaryAttack":
                        ranged.text = t;
                        break;
                }
            }
        }
    }
}