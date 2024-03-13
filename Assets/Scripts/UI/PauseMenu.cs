using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Button resumeButton;

    const string pauseTrigger = "pauseTrigger";
    const string settingsTrigger = "settingsTrigger";

    public void AddResumeListener(UnityAction call)
    {
        resumeButton.onClick.AddListener(call);
    }

    public void Pause()
    {
        animator.SetTrigger(pauseTrigger);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        LevelManager.isGamePaused = true;
        Time.timeScale = 0;
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
        SceneManager.LoadScene(0);
    }

    public void Return()
    {
        animator.SetTrigger(settingsTrigger);
    }
}
