using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button returnButton;
    [SerializeField] Animator animator;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image loadingProgressBar;

    readonly public List<Selectable> controls = new();

    [SerializeField] Selectable[] main;
    [SerializeField] Selectable[] playOptions;
    [SerializeField] Selectable[] levelSelect;
    [SerializeField] Selectable[] settings;
    [SerializeField] Selectable[] tutorial;
    [SerializeField] Selectable[] scrollBars;
    int selected;    
    int currentLevel;
    int currentCheckpoint;
    bool selectingScrollbar;

    const string settingsTrigger = "settingsTrigger";

    ActionMap map;

    private void Awake()
    {
        ChangeSelectables(0);
        map = new ActionMap();
        currentLevel = PlayerPrefs.GetInt(StaticUtilities.CURRENT_LEVEL, 0);
        currentCheckpoint = PlayerPrefs.GetInt(StaticUtilities.CURRENT_CHECKPOINT, 0);
        if (currentLevel == 1)
        {
            if (currentCheckpoint == 0)
            {
                continueButton.interactable = false;
                continueButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.gray;
            }
        }

        map.Menus.Navigate.started += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            if (input.y > 0) SelectVertical(-1);
            else if (input.y < 0) SelectVertical(1);
            else if (input.x > 0) SelectHorizontal(1);
            else if (input.x < 0) SelectHorizontal(-1);
        };

        map.Menus.Submit.started += ctx => 
        {
            Button btn = controls[selected] as Button;
            if (btn)
            {
                btn.onClick.Invoke();
            }
            else if (controls[selected] is Toggle)
            {
                Toggle boggle = (controls[selected] as Toggle);
                boggle.isOn = !boggle.isOn;
            }
        };

        map.General.DeviceTest.performed += ctx => 
        {
            bool usingController = false;
            if (ctx.control.ToString().Contains("Stick")) usingController = true;
            else usingController = false;

            if (usingController)
            {
                Cursor.visible = false;
            }
            else
            {
                Cursor.visible = true;
            }
        };

        map.General.DeviceTest.Enable();
        map.Menus.Enable();
    }

    private void OnDestroy()
    {
        map.Dispose();
    }

    public void PlayLevel(int level)
    {
        PlayerPrefs.DeleteKey(StaticUtilities.CURRENT_LEVEL);
        PlayerPrefs.DeleteKey(StaticUtilities.CURRENT_CHECKPOINT);
        PlayerPrefs.DeleteKey(StaticUtilities.LAST_ENCOUNTER);
        PlayerPrefs.Save();
        StartCoroutine(LoadSceneAsync(level));
    }

    public void Continue()
    {
        StartCoroutine(LoadSceneAsync(currentLevel));
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteKey(StaticUtilities.CURRENT_PLAYER_HEALTH);
        PlayLevel(1);
    }

    public void Settings()
    {
        animator.SetTrigger(settingsTrigger);
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator LoadSceneAsync(int id)
    {
        loadingScreen.SetActive(true);

        yield return new WaitForSeconds(0.25f);
        AsyncOperation op = SceneManager.LoadSceneAsync(id);

        while (!op.isDone)
        {
            float prog = Mathf.Clamp01(op.progress / 0.9f);
            loadingProgressBar.fillAmount = prog;
            yield return null;
        }
    }

    public void ChangeSelectables(int i)
    {
        controls.Clear();
        switch (i)
        {
            case 0:
                controls.AddRange(main);
                break;
            case 1:
                controls.AddRange(playOptions);
                break;
            case 2:
                controls.AddRange(levelSelect);
                break;
            case 3:
                controls.AddRange(settings);
                break;
            case 4:
                controls.AddRange(tutorial);
                break;
        }

        selected = -1;
        SelectVertical(1);
    }

    public void SelectHorizontal(int dir)
    {
        foreach (var bar in scrollBars)
        {
            if (!bar.gameObject.activeInHierarchy) continue;
            
            if (dir == 1)
            {
                bar.Select();
                selectingScrollbar = true;
            }
            else
            {
                selectingScrollbar = false;
                controls[selected].Select();
            }
        }
    }

    public void SelectVertical(int dir)
    {
        if (selectingScrollbar) return;

        do
        {
            selected = selected + dir >= controls.Count ? 0 : selected + dir < 0 ? controls.Count - 1 : selected + dir;
        } while (!controls[selected].interactable);
        controls[selected].Select();
    }
}
