using UnityEngine;
using Cinemachine;

public class Player : DamageableEntity
{
    public PlayerMovement movementScript { get; private set; }
    public PlayerAnimator animatorScript { get; private set; }
    public PlayerMenuController pauseScript { get; private set; }

    

    public GameObject cameraRigPrefab;
    internal CinemachineFreeLook freeLookCam;

    // INPUT
    internal ActionMap actions;

    void Awake()
    {
        freeLookCam = Instantiate(cameraRigPrefab, transform).transform.GetChild(0).GetComponent<CinemachineFreeLook>(); ;
        freeLookCam.LookAt = transform;
        freeLookCam.Follow = transform;

        movementScript = GetComponent<PlayerMovement>();
        animatorScript = GetComponent<PlayerAnimator>();
        pauseScript = GetComponent<PlayerMenuController>();

        actions = new ActionMap();

        // All modules attached to this gameobject
        foreach (IInputExpander module in GetComponents<IInputExpander>())
        {
            module.SetupInputEvents(this, actions);
        }

        SetupInputEvents();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //GameManager.Instance.DisableLobbyCamera();
    }

    public void OnDestroy()
    {
        actions.Dispose();
    }

    public virtual void SetupInputEvents()
    {
        actions.General.Escape.performed += ctx =>
        {
            PausePlayer();
            //pauseScript.Pause();
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
