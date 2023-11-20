using UnityEngine;

public class Player : DamageableEntity
{
    public PlayerMovement movementScript { get; private set; }
    public PlayerCameras cameraScript { get; private set; }
    public PlayerAbilities abilitiesScript { get; private set; }
    public PlayerAnimator animatorScript { get; private set; }
    public HUD hud { get; private set; }
    [SerializeField] private HUD _hud;

    // INPUT
    ActionMap actions;

    private void Start()
    {
        if (!IsOwner) return;

        movementScript = GetComponent<PlayerMovement>();
        cameraScript = GetComponent<PlayerCameras>();
        abilitiesScript = GetComponent<PlayerAbilities>();
        animatorScript = GetComponent<PlayerAnimator>();
        hud = _hud;

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
}
