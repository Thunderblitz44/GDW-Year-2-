
public class Player : DamageableEntity
{
    PlayerMovement movementScript;
    PlayerCameras playerCamerasScript;

    // INPUT
    ActionMap actions;

    private void Start()
    {
        if (!IsOwner) return;

        movementScript = GetComponent<PlayerMovement>();
        playerCamerasScript = GetComponent<PlayerCameras>();

        actions = new ActionMap();

        // All modules attached to this gameobject
        foreach (IInputExpander module in GetComponents<IInputExpander>())
        {
            module.SetupInputEvents(this, actions);
        }

        GameManager.instance.DisableLobbyCamera();
    }

    public override void OnDestroy()
    {
        if (!IsOwner) return; 
        actions.Dispose();
        base.OnDestroy();
    }

    public PlayerCameras GetCameraControllerScript() => playerCamerasScript;
    public PlayerMovement GetMovementScript() => movementScript;
}
