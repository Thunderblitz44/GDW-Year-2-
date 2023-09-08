using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : DamageableEntity, IPlayerStateListener
{
    PlayerMovement movementScript;
    PlayerCameras playerCamerasScript;
    PlayerAbilities abilitiesScript;

    public bool isInCombat { get; private set; }

    // INPUT
    ActionMap actions;

    private void Awake()
    {
        movementScript = GetComponent<PlayerMovement>();
        playerCamerasScript = GetComponent<PlayerCameras>();

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
