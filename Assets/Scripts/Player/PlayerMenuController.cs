using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuController : MonoBehaviour, IInputExpander
{
    Player playerScript;
    ActionMap actions;

    [SerializeField] private PauseMenu pauseMenu;

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        actions.Menus.Resume.performed += ctx => 
        {
            //pauseMenu.Resume(); 
            //playerScript.UnPausePlayer();
        };
        actions.Menus.Select.performed += ctx => { };
        actions.Menus.Confirm.performed += ctx => { };

        pauseMenu.AddResumeListener(playerScript.UnPausePlayer);
        actions.Menus.Disable();
    }
}
