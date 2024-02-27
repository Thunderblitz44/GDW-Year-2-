using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Player : DamageableEntity
{
    public PlayerMovement MovementScript { get; private set; }
    [SerializeField] protected AbilityHUD abilityHud;
    PlayerMenuController pauseScript;

    // INPUT
    protected ActionMap actions;

    [SerializeField] protected CinemachineFreeLook freeLookCam;
    CinemachineInputProvider inputProvider;
    bool usingController = false;
    bool wasUsingController = false;
   

    protected override void Awake()
    {
        base.Awake();

        MovementScript = GetComponent<PlayerMovement>();
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

        inputProvider = freeLookCam.GetComponent<CinemachineInputProvider>();
    }

    private void Start()
    {
        DebugHUD.instance.DisplayControls(actions);
        LevelManager.Instance.CurrentCheckpoint.Teleport(transform);
        hp.SetHealth(PlayerPrefs.GetInt(StaticUtilities.CURRENT_PLAYER_HEALTH, hp.MaxHealth));
    }


    public void OnDestroy()
    {
        actions.Dispose();
    }

    public virtual void SetupInputEvents()
    {
        actions.General.Pause.performed += ctx =>
        {
            PausePlayer();
            MovementScript.Rb.velocity = Vector3.zero;

            actions.Menus.Enable();
            pauseScript.Pause();
        };
        actions.CameraControl.Look.started += ctx =>
        {
            if (ctx.control.ToString().Contains("Stick")) usingController = true;
            else usingController = false;

            if (!usingController && wasUsingController)
            {
                // using mouse
                freeLookCam.m_XAxis.m_MaxSpeed = 0.1f;
                freeLookCam.m_YAxis.m_MaxSpeed = 0.0008f;

                // change the controls panel
                DebugHUD.instance.DisplayControls(actions, true);
            }
            else if (usingController && !wasUsingController)
            {
                // using controller
                freeLookCam.m_XAxis.m_MaxSpeed = 1.2f;
                freeLookCam.m_YAxis.m_MaxSpeed = 0.008f;

                // change the controls panel
                DebugHUD.instance.DisplayControls(actions, false);
            }
            wasUsingController = usingController;
        };
        

        // v TEMPORARY v //
        actions.General.respawnTest.performed += ctx =>
        {
            LevelManager.Instance.Respawn();
        };
        actions.General.resetProgressTest.performed += ctx =>
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(LevelManager.Id);
        };
        actions.General.harmSelfTest.performed += ctx =>
        {
            ApplyDamage(10);
        };
        // ^ TEMPORARY ^ //

        actions.General.Enable();
        actions.CameraControl.Enable();
    }

    public void PausePlayer()
    {
        actions.General.Disable();
        actions.Abilities.Disable();
        MovementScript.DisableLocomotion();
        freeLookCam.gameObject.SetActive(false);
    }

    public void UnPausePlayer()
    {
        actions.General.Enable();
        actions.Abilities.Enable();
        MovementScript.EnableLocomotion();
        freeLookCam.gameObject.SetActive(true);
    }

    protected override void OnHealthZeroed()
    {
        // player death.
        LevelManager.isGameOver = true;
        PausePlayer();
        LevelManager.Instance.Respawn(0.5f);
    }

    public void FreezeCamera()
    {
        freeLookCam.Follow = null;
    }

    public void UnFreezeCamera()
    {
        freeLookCam.Follow = transform;
    }
}
