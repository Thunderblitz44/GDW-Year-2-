using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour, IInputExpander
{

    // DASH
    [SerializeField] float maxDistance = 20f;
    [SerializeField] float force = 10f;
    [SerializeField] float cooldown = 2f;
    bool isDashing = false;
    float dashTime = 0f;
    

    Player playerScript;
    ActionMap actions;

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        actions.Abilities.GrappleAbility.performed += ctx =>
        {

        };
        actions.Abilities.DashAbility.performed += ctx =>
        {
            Rigidbody rb = playerScript.GetMovementScript().GetRigidbody();
            rb.AddForce(transform.forward * force * rb.mass, ForceMode.Impulse);
            isDashing = true;
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


    private void Update()
    {
        if (isDashing)
        {

        }   
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
