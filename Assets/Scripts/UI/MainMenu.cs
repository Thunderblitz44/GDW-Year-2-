using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Animator animator;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image loadingProgressBar;
    int currentLevel;
    int currentCheckpoint;

    const string settingsTrigger = "settingsTrigger";

    private void Awake()
    {
        currentLevel = PlayerPrefs.GetInt(StaticUtilities.CURRENT_LEVEL, 0);
        currentCheckpoint = PlayerPrefs.GetInt(StaticUtilities.CURRENT_CHECKPOINT, 0);
        if (currentLevel == 1)
        {
            if (currentCheckpoint == 0)
            {
                continueButton.interactable = false;
                continueButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.gray;
                return;
            }
        }
    }

    public void PlayLevel(int level)
    {
        PlayerPrefs.DeleteAll();
        StartCoroutine(LoadSceneAsync(level));
    }

    public void Continue()
    {
        StartCoroutine(LoadSceneAsync(currentLevel));
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        StartCoroutine(LoadSceneAsync(1));
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
}
