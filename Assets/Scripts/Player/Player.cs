using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Player : DamageableEntity
{
    public PlayerMovement MovementScript { get; private set; }
    [SerializeField] internal AbilityHUD abilityHud;
    PlayerMenuController pauseScript;

    // INPUT
    internal ActionMap actions;

    [SerializeField] internal CinemachineFreeLook freeLookCam;
   

    internal override void Awake()
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

        hp.SetHealth(PlayerPrefs.GetFloat(StaticUtilities.CURRENT_PLAYER_HEALTH, hp.maxHealth));
    }

    private void Start()
    {
        DebugHUD.instance.DisplayControls(actions);
        LevelManager.Instance.CurrentCheckpoint.Teleport(transform);
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
            actions.Menus.Enable();
            pauseScript.Pause();
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
            ApplyDamage(10f, DamageTypes.physical);
        };
        // ^ TEMPORARY ^ //

        actions.General.Enable();
        actions.CameraControl.Enable();
    }

    public void PausePlayer()
    {
        actions.General.Disable();
        actions.Abilities.Disable();
        actions.Locomotion.Disable();
        freeLookCam.gameObject.SetActive(false);
    }

    public void UnPausePlayer()
    {
        actions.General.Enable();
        actions.Abilities.Enable();
        actions.Locomotion.Enable();
        freeLookCam.gameObject.SetActive(true);
    }

    internal override void OnHealthZeroed()
    {
        // player death.
        LevelManager.isPlayerDead = true;
        PausePlayer();
        LevelManager.Instance.Respawn(0.5f);
    }
}
