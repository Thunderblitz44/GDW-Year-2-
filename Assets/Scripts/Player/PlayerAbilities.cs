using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour, IInputExpander
{

    // DASH
    [SerializeField] float dashDistance = 20f;
    [SerializeField] float force = 10f;
    [SerializeField] float cooldown = 2f;
    bool isDashing = false;
    float dashTime = 0f;

    public float thing = 5f;

    public LayerMask whatIsGrapplable;

    Player playerScript;
    ActionMap actions;
    Rigidbody rb;

    bool isGrappling;

    public Action OnGrappleStarted, OnGrappleEnded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        actions.Abilities.GrappleAbility.performed += ctx =>
        {
            if (isGrappling) return;
            
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, Camera.main.transform.forward),out hit, 50f, whatIsGrapplable))
            {
                playerScript.GetMovementScript().SetToIgnore();
                isGrappling = true;

                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(CalculateLaunchVelocity(transform.position, hit.transform.position) * rb.mass, ForceMode.Impulse);
                //rb.velocity = CalculateLaunchVelocity(transform.position, hit.transform.position);
            }
        };
        actions.Abilities.DashAbility.performed += ctx =>
        {
            rb.AddForce(transform.forward * force * rb.mass, ForceMode.Impulse);
            // raycast - make sure there are no walls
            // if wall, adjust distance
            // lerp
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



    Vector3 CalculateLaunchVelocity(Vector3 startpoint, Vector3 endpoint)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endpoint.y - startpoint.y;
        float h = displacementY + thing;
        Vector3 displacementXZ = new Vector3(endpoint.x - startpoint.x, 0f, endpoint.z - startpoint.z);

        Vector3 velocityY = Vector3.up * MathF.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / (MathF.Sqrt(-2 * h / gravity) 
            + MathF.Sqrt(2 * (displacementY - h) / gravity));
        return velocityXZ + velocityY;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (isGrappling)
        {
            isGrappling = false;
            playerScript.GetMovementScript().AutoDetectState();
            Debug.Log("grapple finished");
        }
    }

    private void Update()
    {
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
