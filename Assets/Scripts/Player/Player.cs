using UnityEngine;
using Cinemachine;

public class Player : DamageableEntity
{
    public PlayerMovement movementScript { get; private set; }
    public PlayerAnimator animatorScript { get; private set; }
    public PlayerMenuController pauseScript { get; private set; }
    public AbilityHUD abilityHud;

    // INPUT
    internal ActionMap actions;

    [SerializeField] internal CinemachineFreeLook freeLookCam;
    //[SerializeField] internal CinemachineFreeLook aimCam;

    internal override void Awake()
    {
        base.Awake();

        movementScript = GetComponent<PlayerMovement>();
        animatorScript = GetComponent<PlayerAnimator>();
        pauseScript = GetComponent<PlayerMenuController>();

        actions = new ActionMap();

        StaticUtilities.playerTransform = transform;

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
