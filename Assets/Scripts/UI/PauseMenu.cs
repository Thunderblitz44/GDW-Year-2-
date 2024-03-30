using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Button resumeButton;
    [SerializeField] EventSystem eventSys;

    readonly public List<Selectable> controls = new();

    [SerializeField] Selectable[] main;
    [SerializeField] Selectable[] settings;
    [SerializeField] Selectable[] tutorial;
    [SerializeField] Selectable[] scrollBars;

    int selected;
    bool selectingScrollbar;

    const string pauseTrigger = "pauseTrigger";
    const string settingsTrigger = "settingsTrigger";

    private void Awake()
    {
    }

    public void AddResumeListener(UnityAction call)
    {
        resumeButton.onClick.AddListener(call);
    }

    public void Pause()
    {
        animator.SetTrigger(pauseTrigger);
        LevelManager.isGamePaused = true;
        Time.timeScale = 0;
        ChangeSelectables(0);
    }

    public void Resume()
    {
        animator.SetTrigger(pauseTrigger);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        LevelManager.isGamePaused = false;
        Time.timeScale = 1;
    }

    public void Settings()
    {
        animator.SetTrigger(settingsTrigger);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        LevelManager.Instance.LoadNextLevel(true);
    }

    public void Return()
    {
        animator.SetTrigger(settingsTrigger);
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
                controls.AddRange(settings);
                break;
            case 2:
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

    public void Submit()
    {
        if (controls[selected] is Button)
        {
            (controls[selected] as Button).onClick.Invoke();
        }
        else if (controls[selected] is Toggle)
        {
            Toggle boggle = (controls[selected] as Toggle);
            boggle.isOn = !boggle.isOn;
        }
    }
}
