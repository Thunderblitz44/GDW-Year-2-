using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : DamageableEntity
{
    PlayerMovement movementScript;
    PlayerCameras playerCamerasScript;

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

    public PlayerCameras GetCameraControllerScript() => playerCamerasScript;
    public PlayerMovement GetMovementScript() => movementScript;
}
