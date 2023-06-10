using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour, IInputExpander
{
    [SerializeField] float reach;
    [SerializeField] float showInfoAngle;
    [SerializeField] LayerMask whatIsInteractable;
    [SerializeField] Transform holdPos;

    ActionMap actions;
    List<GameObject> itemsInReach;

    // LOOKING AT OBJECTS
    bool lookingAtObj;
    Item item;
    bool isInspectingObject;
    GameObject grabbedItem;

    // MESSAGES
    [SerializeField] string pickupMessage = "[<b>E</b>] <i><color=yellow>Pickup</color></i>";
    [SerializeField] string inspectMessage = "[<b>I</b>] <i><color=#D2D2D2>Inspect</color><i>";
    [SerializeField] string unInspectMessage = "[<b>I</b>] <i><color=#D2D2D2>Show Less</color><i>";


    // REFERENCE TO PLAYER
    Player player;

    #region Unity Messages

    private void Start()
    {
        itemsInReach = new();
    }

    // detecting interactables
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            itemsInReach.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Item")
        {
            itemsInReach.Remove(other.gameObject);
            OnLookAwayFromInteractable();
        }
    }

    private void Update()
    {
        // if we are within reach of an item
        if (itemsInReach.Count > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hit, reach, whatIsInteractable, QueryTriggerInteraction.Ignore))
            {
                if (!lookingAtObj)
                {
                    // Is looking at object
                    OnLookAtInteractable(hit.collider.gameObject);
                }
            }
            else if (lookingAtObj)
            {
                // looked away
                OnLookAwayFromInteractable();
            }
        }

        if (grabbedItem)
        {
            grabbedItem.transform.position = holdPos.position;
            grabbedItem.transform.rotation = player.GetMovementScript().GetOrientation().rotation;
        }
    }

    #endregion


    public void OnLookAtInteractable(GameObject obj)
    {
        lookingAtObj = true;

        if (obj.TryGetComponent(out item))
        {
            player.GetHUDScript().DisplayText(HUD.TextArea.textAboveCrosshair, item.name);
            player.GetHUDScript().SetTextAreaVisibility(HUD.TextArea.textAboveCrosshair, true);

            string belowCrosshairText = "";
            if (item.IsInteractable()) belowCrosshairText += $"{pickupMessage}\n";
            if (item.IsInspecable())
            {
                belowCrosshairText += inspectMessage;
            }

            player.GetHUDScript().DisplayText(HUD.TextArea.textBelowCrosshair, belowCrosshairText);
            player.GetHUDScript().SetTextAreaVisibility(HUD.TextArea.textBelowCrosshair, true);
        }
    }

    public void OnLookAwayFromInteractable()
    {
        lookingAtObj = false;

        if (item)
        {
            player.GetHUDScript().SetTextRegionVisibility(HUD.TextRegion.crosshair, false);
            item = null;
        }
    }


    void Interact()
    {
        // pickup
        if (!grabbedItem)
        {
            if (!item) return;
            if (!item.IsInteractable()) return;

            grabbedItem = item.gameObject;
            item.Pickup();
        }
        else
        {
            grabbedItem.GetComponent<Item>().Drop();
            grabbedItem = null;
        }
    }

    void Inspect()
    {
        if (!item) return;
        if (!item.IsInspecable()) return;

        isInspectingObject = !isInspectingObject;

        string belowCrosshairText = "";
        if (item.IsInteractable()) belowCrosshairText += $"{pickupMessage}\n";
        
        // toggling the inspection window
        if (isInspectingObject)
        {
            // show and populate inspector window
            player.GetHUDScript().DisplayText(HUD.TextArea.textRightOfCrosshair, item.GetProperties());
            player.GetHUDScript().SetTextAreaVisibility(HUD.TextArea.textRightOfCrosshair, true);
            belowCrosshairText += unInspectMessage;
        }
        else
        {
            player.GetHUDScript().SetTextAreaVisibility(HUD.TextArea.textRightOfCrosshair, false);
            belowCrosshairText += inspectMessage;
        }

        player.GetHUDScript().DisplayText(HUD.TextArea.textBelowCrosshair, belowCrosshairText);
    }

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        this.actions = actions;
        player = (Player)sender;

        // Interact / pickup
        actions.Interaction.Interact.started += ctx =>
        {
            Interact();
        };

        // Held interact
        actions.Interaction.Interact.performed += ctx =>
        {
            Debug.Log("hold");

        };

        
        // Inspect
        actions.Interaction.Inspect.started += ctx =>
        {
            Inspect();
        };



        EnableInteractions();
    }

    public void EnableInteractions() => actions.Interaction.Enable();
    public void DisableInteractions() => actions.Interaction.Disable();
}
