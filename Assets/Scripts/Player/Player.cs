using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayerStateListener
{
    [Header("Objects")]
    [SerializeField] PlayerMovement movementScript;
    [SerializeField] PlayerInteraction interactionScript;
    [SerializeField] PlayerCameras playerCamerasScript;
    [SerializeField] PlayerAbilities abilitiesScript;
    [SerializeField] HUD hudScript;

    public bool isInCombat { get; private set; }

    // INPUT
    ActionMap actions;

    private void Awake()
    {
        actions = new ActionMap();

        // All modules attached to this gameobject
        foreach (IInputExpander module in GetComponents<IInputExpander>())
        {
            module.SetupInputEvents(this, actions);
        }
    }

    private void OnDestroy()
    {
        actions.Dispose();
    }

    public void SetIsInCombat(bool isInCombat)
    {
        foreach (IPlayerStateListener listener in GetComponents<IPlayerStateListener>())
        {
            if (isInCombat)
            {
                listener.SetCombatState();
            }
            else
            {
                listener.SetFreeLookState();
            }
        }
    }

    public PlayerCameras GetCameraControllerScript() => playerCamerasScript;
    public PlayerMovement GetMovementScript() => movementScript;
    public PlayerInteraction GetInteractionScript() => interactionScript;
    public HUD GetHUDScript() => hudScript;
    public ActionMap GetActionMap() => actions;

    public void SetCombatState()
    {
        isInCombat = true;
    }

    public void SetFreeLookState()
    {
        isInCombat = false;
    }
}
