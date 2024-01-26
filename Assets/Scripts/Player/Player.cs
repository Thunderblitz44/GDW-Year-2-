using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Player : DamageableEntity
{
    public PlayerMovement MovementScript { get; private set; }
    public PlayerAnimator AnimatorScript { get; private set; }
    public PlayerMenuController PauseScript { get; private set; }
    public AbilityHUD abilityHud;

    // INPUT
    internal ActionMap actions;

    [SerializeField] internal CinemachineFreeLook freeLookCam;

    internal override void Awake()
    {
        base.Awake();

        MovementScript = GetComponent<PlayerMovement>();
        AnimatorScript = GetComponent<PlayerAnimator>();
        PauseScript = GetComponent<PlayerMenuController>();

        actions = new ActionMap();

        // All modules attached to this gameobject
        foreach (IInputExpander module in GetComponents<IInputExpander>())
        {
            module.SetupInputEvents(this, actions);
        }

        SetupInputEvents();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnDestroy()
    {
        actions.Dispose();
    }

    public virtual void SetupInputEvents()
    {
        actions.General.Escape.performed += ctx =>
        {
            //PausePlayer();
        };
        actions.General.respawnTest.performed += ctx =>
        {
            LevelManager.Instance.Respawn();
        };
        actions.General.resetProgressTest.performed += ctx =>
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(LevelManager.Id);
        };

        actions.General.Enable();
    }

    public void PausePlayer()
    {
        actions.General.Disable();
        actions.Locomotion.Disable();
        actions.CameraControl.Disable();
        actions.Abilities.Disable();
        actions.Menus.Enable();
    }

    public void UnPausePlayer()
    {
        actions.General.Enable();
        actions.Locomotion.Enable();
        actions.CameraControl.Enable();
        actions.Abilities.Enable();
        actions.Menus.Disable();
    }

}
