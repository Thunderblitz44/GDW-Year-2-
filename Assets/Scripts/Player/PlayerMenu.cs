using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PlayerMenu : MonoBehaviour
{

    private ActionMap actions;
    private Animator menuAnimator;

    private void Start()
    {
        actions = new ActionMap();
        actions.Enable();
        actions.General.Escape.performed += OnEscape;
      
        menuAnimator = GetComponentInChildren<Animator>();

        if (menuAnimator == null)
        {
            Debug.LogError("Menu Animator not found!");
        }
    }

    private void OnEscape(InputAction.CallbackContext context)
    {
        if (menuAnimator != null && context.performed)
        {
            menuAnimator.SetTrigger("pauseTrigger");
        }
    }

    private void OnDisable()
    {
        actions.General.Escape.performed -= OnEscape;
    }

    public void ResumeButton()
    {
        menuAnimator.SetTrigger("pauseTrigger");

    }


    public void SettingsButton()
    {
      
        menuAnimator.SetTrigger("settingsTrigger");
    
     
    }
    
    public void ExitButton()
    {
#if UNITY_EDITOR
       
        UnityEditor.EditorApplication.isPlaying = false;
#else
  
        Application.Quit();
#endif
    }
}
