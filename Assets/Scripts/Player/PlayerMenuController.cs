using UnityEngine;

public class PlayerMenuController : MonoBehaviour, IInputExpander
{
    Player playerScript;
    ActionMap actions;
    [SerializeField] PauseMenu pauseMenu;

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        actions.Menus.Return.performed += ctx => 
        {
            Return();
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

        pauseMenu.AddResumeListener(Return);
        actions.Menus.Disable();
    }

    void Return()
    {
        pauseMenu.Resume();
        actions.Menus.Disable();
        playerScript.UnPausePlayer();
        pauseMenu.Save();
    }

    public void Pause()
    {
        pauseMenu.Pause();
    }
}
