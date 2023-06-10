using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour, IInputExpander
{
    Player playerScript;
    ActionMap actions;

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        actions.Abilities.GrappleAbility.performed += ctx =>
        {
            Debug.Log("Grapple");
            playerScript.SetIsInCombat(!playerScript.isInCombat);
            // ray cast
            // hit?
            // check if is either:
            // enemy, movableObject or immovableObject

        };
        actions.Abilities.DashAbility.performed += ctx =>
        {
            // max 3 dashes
            // impulse

        };
        actions.Abilities.ShieldAbility.performed += ctx =>
        {
            // stop moving
            // no dmg
        };
        actions.Abilities.BuffAbility.performed += ctx =>
        {
            // increase damage
            // increase damage resistance
            // last x seconds
        };

        EnableAllAbilities();
    }



    public void EnableAllAbilities() => actions.Abilities.Enable();
    public void DisableAllAbilities() => actions.Abilities.Disable();

    public void EnableGrappleAbility() => actions.Abilities.GrappleAbility.Enable();
    public void DisableGrappleAbility() => actions.Abilities.GrappleAbility.Disable();

    public void EnableDashAbility() => actions.Abilities.DashAbility.Enable();
    public void DisableDashAbility() => actions.Abilities.DashAbility.Disable();

    public void EnableShieldAbility() => actions.Abilities.ShieldAbility.Enable();
    public void DisableShieldAbility() => actions.Abilities.ShieldAbility.Disable();

    public void EnableBuffAbility() => actions.Abilities.BuffAbility.Enable();
    public void DisableBuffAbility() => actions.Abilities.BuffAbility.Disable();
}
