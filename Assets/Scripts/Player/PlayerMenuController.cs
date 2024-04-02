using UnityEngine;

public class PlayerMenuController : MonoBehaviour, IInputExpander
{
    Player playerScript;

    [SerializeField] PauseMenu pauseMenu;

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;

        actions.Menus.Return.performed += ctx => 
        {
            pauseMenu.Resume(); 
            actions.Menus.Disable();
            playerScript.UnPausePlayer();
        };

        actions.Menus.Navigate.started += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            if (input.y > 0) pauseMenu.SelectVertical(-1);
            else if (input.y < 0) pauseMenu.SelectVertical(1);
            else if (input.x > 0) pauseMenu.SelectHorizontal(1);
            else if (input.x < 0) pauseMenu.SelectHorizontal(-1);
        };

        actions.Menus.Submit.started += ctx =>
        {
            pauseMenu.Submit();
        };

        pauseMenu.AddResumeListener(playerScript.UnPausePlayer);
        actions.Menus.Disable();
    }

    public void Pause()
    {
        pauseMenu.Pause();
    }
}
