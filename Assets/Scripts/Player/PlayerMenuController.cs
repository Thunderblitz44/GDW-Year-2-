using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuController : MonoBehaviour, IInputExpander
{
    Player playerScript;

    [SerializeField] PauseMenu pauseMenu;

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;

        actions.Menus.Resume.performed += ctx => 
        {
            pauseMenu.Resume(); 
            actions.Menus.Disable();
            playerScript.UnPausePlayer();
        };
        actions.Menus.Select.performed += ctx => { };
        actions.Menus.Confirm.performed += ctx => { };

        pauseMenu.AddResumeListener(playerScript.UnPausePlayer);
        actions.Menus.Disable();
    }

    public void Pause()
    {
        pauseMenu.Pause();
    }
}
