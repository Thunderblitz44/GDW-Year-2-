using UnityEngine;

public class Player : DamageableEntity
{
    public PlayerMovement movementScript { get; private set; }
    public PlayerCameras cameraScript { get; private set; }
    public PlayerAbilities abilitiesScript { get; private set; }
    public PlayerAnimator animatorScript { get; private set; }
    public PlayerMenuController pauseScript { get; private set; }
    public HUD hud { get; private set; }
    [SerializeField] private HUD _hud;

    // INPUT
    internal ActionMap actions;

    internal virtual void Start()
    {
        if (!IsOwner) return;
        movementScript = GetComponent<PlayerMovement>();
        cameraScript = GetComponent<PlayerCameras>();
        abilitiesScript = GetComponent<PlayerAbilities>();
        animatorScript = GetComponent<PlayerAnimator>();
        pauseScript = GetComponent<PlayerMenuController>();
        hud = _hud;
        hud.pauseMenu.AddResumeListener(UnPausePlayer);

        actions = new ActionMap();

        // All modules attached to this gameobject
        foreach (IInputExpander module in GetComponents<IInputExpander>())
        {
            module.SetupInputEvents(this, actions);
        }

        SetupInputEvents();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.Instance.DisableLobbyCamera();
    }

    public override void OnDestroy()
    {
        if (!IsOwner) return; 
        actions.Dispose();
        base.OnDestroy();
    }

    public virtual void SetupInputEvents()
    {
        actions.General.Escape.performed += ctx =>
        {
            PausePlayer();
            hud.pauseMenu.Pause();
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
