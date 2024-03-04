using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro.EditorUtilities;

public class Player : DamageableEntity
{
    public PlayerMovement MovementScript { get; private set; }
    [SerializeField] protected AbilityHUD abilityHud;
    PlayerMenuController pauseScript;

    // INPUT
    protected ActionMap actions;

    [SerializeField] protected CinemachineFreeLook freeLookCam;
    bool usingController = false;
    bool wasUsingController = false;

    [Header("Auto Lockon")]
    [SerializeField] protected bool autoLock = false;
    [SerializeField] float autoLockRadius = 100f;
    [SerializeField] float autoLockRange = 20f;
    [SerializeField] Image lockonIcon;
    [SerializeField] LayerMask blockingSightLayers;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float lerpSpeed = 5;
    [SerializeField] float targetsCheckDelay = 0.2f;
    protected Transform lockonTarget;
    Vector3 lerpStart;
    Vector3 lerpEnd;
    float lerpTime = 1;
    float targetsChecktimer;
    protected bool autoLockOverride;
    protected float autoLockRadiusOverride;
    protected float autoLockRangeOverride;
    
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
    }

    void Start()
    {
        DebugHUD.instance.DisplayControls(actions);
        LevelManager.Instance.CurrentCheckpoint.Teleport(transform);
        hp.SetHealth(PlayerPrefs.GetInt(StaticUtilities.CURRENT_PLAYER_HEALTH, hp.MaxHealth));
    }

    protected virtual void Update()
    {
        if (autoLock || autoLockOverride)
        {
            SetLockonIconPosition();

            targetsChecktimer += Time.deltaTime;
            if (targetsChecktimer > targetsCheckDelay)
            {
                targetsChecktimer = 0f;
                CheckLockonTargets();
            }
        }
        else if (!autoLock && lockonIcon.isActiveAndEnabled)
        {
            lockonIcon.enabled = false;
        }
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
                
                freeLookCam.m_XAxis.m_MaxSpeed = SettingsManager.Instance.Settings.MouseSensXForCinemachine;
                freeLookCam.m_YAxis.m_MaxSpeed = SettingsManager.Instance.Settings.MouseSensYForCinemachine;

                // change the controls panel
                DebugHUD.instance.DisplayControls(actions, true);
            }
            else if (usingController && !wasUsingController)
            {
                // using controller
                freeLookCam.m_XAxis.m_MaxSpeed = SettingsManager.Instance.Settings.GamepadSensXForCinemachine;
                freeLookCam.m_YAxis.m_MaxSpeed = SettingsManager.Instance.Settings.GamepadSensYForCinemachine;

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
        Destroy(hp); 
    }

    public void FreezeCamera()
    {
        freeLookCam.Follow = null;
    }

    public void UnFreezeCamera()
    {
        freeLookCam.Follow = transform;
    }

    private void CheckLockonTargets()
    {
        List<Transform> targets = StaticUtilities.renderedEnemies;
        SortTargets(ref targets);

        if (targets.Count > 0)
        {
            // is it the same?
            if (lockonTarget == targets[0] && lockonIcon.isActiveAndEnabled) return;
            // are we switching?
            else if (lockonTarget && targets.Count > 1)
            {
                lerpTime = 0f;
                lerpStart = lockonIcon.transform.position;
            }
            else if (!lockonTarget) lerpTime = 1;

            // if its the first / change
            lockonTarget = targets[0];
            OnLockonTargetChanged();
        }
        else if (lockonIcon.isActiveAndEnabled)
        {
            lockonIcon.enabled = false;
            lockonTarget = null;
            OnLockonTargetChanged();
        }
    }

    public void SortTargets(ref List<Transform> targets)
    {
        // sort by distance
        targets = StaticUtilities.SortByDistanceToScreenCenter(targets, autoLockOverride ? autoLockRadiusOverride : autoLockRadius);

        // sort by visible
        targets = StaticUtilities.SortByVisible(targets, autoLockOverride ? autoLockRangeOverride : autoLockRange, targetLayer, blockingSightLayers);
    }

    private void SetLockonIconPosition()
    {
        if (lockonTarget)
        {
            lerpEnd = Camera.main.WorldToScreenPoint(lockonTarget.position);

            if (lerpTime < 1)
            {
                lockonIcon.transform.position = Vector2.Lerp(lerpStart, lerpEnd, StaticUtilities.easeCurve01.Evaluate(lerpTime));
                lerpTime += Time.deltaTime * lerpSpeed;
            }
            else
            {
                lockonIcon.transform.position = lerpEnd;
            }

            if (!lockonIcon.isActiveAndEnabled) lockonIcon.enabled = true;
        }
    }

    protected virtual void OnLockonTargetChanged()
    {

    }
}
