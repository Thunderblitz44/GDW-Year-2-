using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.VFX;

public abstract class Player : DamageableEntity
{
    public PlayerMovement MovementScript { get; private set; }
    [SerializeField] GameSettings activeSettings;
    [SerializeField] protected AbilityHUD abilityHud;
    PlayerMenuController pauseScript;

    [SerializeField] private VisualEffect onHitEffects;
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
    [SerializeField] float iconLerpSpeed = 5;
    [SerializeField] float targetsCheckDelay = 0.2f;
    protected Transform lockonTarget;
    Vector3 lerpStart;
    Vector3 lerpEnd;
    float lerpTime = 1;
    float targetsChecktimer;
    protected bool autoLockOverride;
    protected float autoLockRadiusOverride;
    protected float autoLockRangeOverride;


    // camera shake
    CinemachineBasicMultiChannelPerlin[] noise;
    float shakeSpeed = 1;

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

        noise = new CinemachineBasicMultiChannelPerlin[3]
        {
            freeLookCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(),
            freeLookCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(),
            freeLookCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>()
        };
    }

    void Start()
    {
        if (DebugHUD.instance) DebugHUD.instance.DisplayControls(actions);
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

        // update camera frustrum planes
        StaticUtilities.cameraFrustrumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);


        for (int i = 0; i < 3; i++)
        {
            noise[i].m_AmplitudeGain = Mathf.Clamp(noise[i].m_AmplitudeGain - Time.deltaTime * shakeSpeed, 0, 100f);
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
            actions.Menus.Enable();
            pauseScript.Pause();
            if (!usingController)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        };
        actions.General.DeviceTest.performed += ctx =>
        {
            if (ctx.control.ToString().Contains("Stick")) usingController = true;
            else usingController = false;

            if (!usingController && wasUsingController)
            {
                // using mouse
                freeLookCam.m_XAxis.m_MaxSpeed = activeSettings.MouseSensXForCinemachine;
                freeLookCam.m_YAxis.m_MaxSpeed = activeSettings.MouseSensYForCinemachine;

                if (LevelManager.isGamePaused)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }

                // change the controls panel
                DebugHUD.instance.DisplayControls(actions, true);
            }
            else if (usingController && !wasUsingController)
            {
                // using controller
                freeLookCam.m_XAxis.m_MaxSpeed = activeSettings.GamepadSensXForCinemachine;
                freeLookCam.m_YAxis.m_MaxSpeed = activeSettings.GamepadSensYForCinemachine;
                
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

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
            PlayerPrefs.DeleteKey(StaticUtilities.CURRENT_LEVEL);
            PlayerPrefs.DeleteKey(StaticUtilities.CURRENT_CHECKPOINT);
            PlayerPrefs.DeleteKey(StaticUtilities.LAST_ENCOUNTER);
            PlayerPrefs.DeleteKey(StaticUtilities.CURRENT_PLAYER_HEALTH);
            SceneManager.LoadScene(LevelManager.Id);
        };
        // ^ TEMPORARY ^ //

        actions.General.Enable();
    }

    public void PausePlayer()
    {
        actions.General.respawnTest.Disable();
        actions.General.Pause.Disable();
        actions.General.resetProgressTest.Disable();
        actions.Abilities.Disable();
        MovementScript.DisableLocomotion();
        freeLookCam.gameObject.SetActive(false);
    }

    public void UnPausePlayer()
    {
        actions.Menus.Disable();
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
        MovementScript.Death();
        LevelManager.Instance.Respawn(1f);
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
        targets = StaticUtilities.SortByVisible(targets, autoLockOverride ? autoLockRangeOverride : autoLockRange, blockingSightLayers);
    }

    private void SetLockonIconPosition()
    {
        if (lockonTarget)
        {
            lerpEnd = Camera.main.WorldToScreenPoint(lockonTarget.position);

            if (lerpTime < 1)
            {
                lockonIcon.transform.position = Vector2.Lerp(lerpStart, lerpEnd, StaticUtilities.easeCurve01.Evaluate(lerpTime));
                lerpTime += Time.deltaTime * iconLerpSpeed;
            }
            else
            {
                lockonIcon.transform.position = lerpEnd;
            }

            if (!lockonIcon.isActiveAndEnabled) lockonIcon.enabled = true;
        }
    }

    public void DoCameraShake(float amp, float freq, float speed = 1)
    {
        foreach (var item in noise)
        {
            item.m_AmplitudeGain = amp;
            item.m_FrequencyGain = freq;
        }
        shakeSpeed = speed;
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);
        if (damage > 0)
        {
            onHitEffects.SendEvent("Blood");
        }
        
        DoCameraShake(2,1,12);
    }

    public void SettingsChanged()
    {
        if (usingController)
        {
            freeLookCam.m_YAxis.m_MaxSpeed = activeSettings.GamepadSensYForCinemachine;
            freeLookCam.m_XAxis.m_MaxSpeed = activeSettings.GamepadSensXForCinemachine;
        }
        else
        {
            freeLookCam.m_XAxis.m_MaxSpeed = activeSettings.MouseSensXForCinemachine;
            freeLookCam.m_YAxis.m_MaxSpeed = activeSettings.MouseSensYForCinemachine;
        }

        autoLock = activeSettings.autoLock;
    }

    protected abstract void OnLockonTargetChanged();
}
