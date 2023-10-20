using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMenu : MonoBehaviour
{
    public GameObject overlayUiPrefab;
    private ActionMap actions;
    private Animator menuAnimator;
    private Animator uiOverLayAnimator;

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

    public void InstantiateOverlay()
    {
        uiOverLayAnimator = overlayUiPrefab.GetComponent<Animator>();
    }
    public void OnEscape(InputAction.CallbackContext context)
    {
        
        if (menuAnimator != null && context.performed)
        {
            uiOverLayAnimator.SetTrigger("emberTrigger");
           
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
