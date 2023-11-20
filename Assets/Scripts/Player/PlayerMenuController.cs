using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuController : MonoBehaviour, IInputExpander
{
    Player playerScript;
    ActionMap actions;

    PauseMenu menu;

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        menu = playerScript.hud.pauseMenu;

        actions.Menus.Resume.performed += ctx => 
        {
            menu.Resume(); 
            playerScript.UnPausePlayer();
        };
        actions.Menus.Select.performed += ctx => { };
        actions.Menus.Confirm.performed += ctx => { };

        actions.Menus.Disable();
    }
}
