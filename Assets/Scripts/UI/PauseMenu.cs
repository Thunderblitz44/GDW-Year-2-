using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Button resumeButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button quitButton;
    //[SerializeField] Button mainMenuButton;
    [SerializeField] Button returnButton;

    [SerializeField] Animator animator;

    const string pauseTrigger = "pauseTrigger";
    const string settingsTrigger = "settingsTrigger";

    private void Start()
    {
        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(Settings);
        quitButton.onClick.AddListener(Exit);
        //mainMenuButton.onClick.AddListener(MainMenu);
        returnButton.onClick.AddListener(Return);
    }

    private void OnDestroy()
    {
        resumeButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        //mainMenuButton.onClick.RemoveAllListeners();
        returnButton.onClick.RemoveAllListeners();
    }

    public void AddResumeListener(UnityAction call)
    {
        resumeButton.onClick.AddListener(call);
    }

    public void Pause()
    {
        animator.SetTrigger(pauseTrigger);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        animator.SetTrigger(pauseTrigger);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Settings()
    {
        animator.SetTrigger(settingsTrigger);
    }

    void Exit()
    {

    }

    void MainMenu()
    {
        
    }

    void Return()
    {

    }
}
