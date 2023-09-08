using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour, IInputExpander
{
    // DASH
    [SerializeField] float dashDistance = 20f;
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] AnimationCurve dashCurve;
    bool isDashing = false;

    public float thing = 5f;

    public LayerMask whatIsDashObstacle;
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
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,out hit, 50f, whatIsGrapplable))
            {
                playerScript.GetMovementScript().Disable();
                isGrappling = true;

                
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(CalculateLaunchVelocity(transform.position, hit.transform.position) * rb.mass, ForceMode.Impulse);
            }
        };
        actions.Abilities.DashAbility.performed += ctx =>
        {
            if (isDashing) return;
            isDashing = true;
            playerScript.GetMovementScript().Disable();

            // raycast - make sure there are no obstacles in the way
            float newDist = dashDistance;

            Vector3 end;
            if (rb.velocity.magnitude > 0) end = transform.position + rb.velocity.normalized * newDist;
            else { end = Camera.main.transform.forward; end.y = 0; end += transform.position; }

            // slight issue here - fix the cameras first
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (transform.position - end).normalized, 
                out hit, dashDistance, whatIsDashObstacle))
            {
                newDist = Vector3.Distance(hit.point, transform.position) - 0.7f;
            }

            // lerp
            if (rb.velocity.magnitude > 0) end = transform.position + rb.velocity.normalized * newDist;
            else { end = Camera.main.transform.forward; end.y = 0; end += transform.position; }

            Debug.DrawRay(transform.position, end - transform.position, Color.red, 3f);

            dashCurve.ClearKeys();
            dashCurve.AddKey(0,0);
            dashCurve.AddKey(newDist / dashSpeed, 1);

            StartCoroutine(DashRoutine(end));
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

    IEnumerator DashRoutine(Vector3 endPos)
    {
        Vector3 startPos = transform.position;
        float time = 0;
        float dashTime = dashCurve.keys[1].time;

        while (time < dashTime)
        {
            transform.position = Vector3.Lerp(startPos,endPos, dashCurve.Evaluate(time += Time.deltaTime));

            yield return null;
        }

        playerScript.GetMovementScript().Enable();
        isDashing = false;
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
            playerScript.GetMovementScript().Enable();
            Debug.Log("grapple finished");
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
