using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Percy : Player
{
    [Space(10), Header("ABILITIES"), Space(10)]
    [Header("Grapple")]
    [SerializeField] float grappleDistance = 50f;
    [SerializeField] float lightWeightMaxMass = 1f;
    [SerializeField] float overshoot = 5f;
    [SerializeField] float playerLandingSpace = 1f;
    [SerializeField] float meetInTheMiddleSpacing = 0.5f;
    [SerializeField] float itemLandingSpace = 1f;
    [SerializeField] float grappleCooldown = 3f;
    [SerializeField] LayerMask whatIsGrapplable;
    [SerializeField] GameObject grappleMeterPrefab;
    [SerializeField] Image grappleLockIcon;
    [SerializeField] float lerpSpeed = 4f;
    bool isGrappling = false;
    bool grappleReady = true;
    GrappleUI grappleUI;

    Transform lockedTarget;
    Vector3 lockLerpStart;
    AnimationCurve lerpCurve = AnimationCurve.EaseInOut(0,0,1,1);



    float lerpTime;
    float timer;

    float targetsCheckDelay = 0.1f;

    Player playerScript;
    Rigidbody rb;

    internal override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();

        grappleUI = Instantiate(grappleMeterPrefab, GameManager.Instance.canvas).GetComponent<GrappleUI>();
        grappleUI.onGrappleRecharged += OnGrappleRecharged;
    }

    private void Update()
    {
        if (!IsOwner) return;

        // slerp lock icon
        SetLockonIconPosition();

        timer += Time.deltaTime;
        if (timer < targetsCheckDelay) return;
        timer = 0f;

        CheckGrappleTargets();
    }

    public override void SetupInputEvents()
    {
        base.SetupInputEvents();

        // Percy's Abilities

        // BASIC
        actions.General.Attack.performed += ctx =>
        {
            //GameObject.Find("TestDummy").GetComponent<IDamageable>().ApplyDamage(1f, DamageTypes.physical);
        };
        actions.CameraControl.Aim.performed += ctx =>
        {
            //GameObject.Find("TestDummy").GetComponent<IDamageable>().ApplyDamage(-1f, DamageTypes.magic);
        };


        // Abilities

        // GRAPPLE
        actions.Abilities.First.started += ctx => 
        {
            if (isGrappling || !grappleReady || !lockedTarget) return;

            grappleUI.SpendPoint();
            Rigidbody targetRb = lockedTarget.GetComponent<Rigidbody>();

            Vector3 launchForce = Vector3.zero;
            Vector3 dir = (lockedTarget.position - transform.position).normalized;

            // process the grapple target
            if (!targetRb)
            {
                // grapple point - we go to it
                float dist = Vector3.Distance(transform.position, lockedTarget.position) - playerLandingSpace;
                Vector3 playerEndPoint = transform.position + dir * dist;
                launchForce = StaticUtilities.CalculateLaunchVelocity(transform.position, playerEndPoint, overshoot);
            }
            else if (targetRb.mass > lightWeightMaxMass)
            {
                // something heavy, meet in the middle
                float dist = Vector3.Distance(transform.position, lockedTarget.position) / 2 - meetInTheMiddleSpacing;
                Vector3 playerEndPoint = transform.position + dir * dist;
                Vector3 targetEndPoint = lockedTarget.position - dir * dist;

                launchForce = StaticUtilities.CalculateLaunchVelocity(transform.position, playerEndPoint, overshoot);

                // tell the server to launch the target
                LaunchTargetServerRpc(lockedTarget.GetComponent<NetworkObject>().NetworkObjectId, StaticUtilities.CalculateLaunchVelocity(lockedTarget.position, targetEndPoint, overshoot) * targetRb.mass);
            }
            else
            {
                // light object, it comes to us
                float dist = Vector3.Distance(transform.position, lockedTarget.position) - itemLandingSpace;
                Vector3 targetEndPoint = lockedTarget.position - dir * dist;

                // tell the server to launch the target
                LaunchTargetServerRpc(lockedTarget.GetComponent<NetworkObject>().NetworkObjectId, StaticUtilities.CalculateLaunchVelocity(lockedTarget.position, targetEndPoint, overshoot) * targetRb.mass);
                goto cooldown;
            }

            isGrappling = true;
            playerScript.movementScript.Disable();
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(launchForce * rb.mass, ForceMode.Impulse);

        cooldown:
            grappleReady = false;
            grappleUI.RechargePoint(grappleCooldown);
        };

        actions.Abilities.Second.started += ctx => { };
        actions.Abilities.Second.canceled += ctx => { };

        actions.Abilities.Third.started += ctx => { };
        actions.Abilities.Third.canceled += ctx => { };

        actions.Abilities.Fourth.started += ctx => { };
        actions.Abilities.Fourth.canceled += ctx => { };
        
        actions.Abilities.Enable();
    }

    [ServerRpc(RequireOwnership = false)]
    void LaunchTargetServerRpc(ulong networkObjectId, Vector3 force)
    {
        NetworkObject no = NetworkManager.SpawnManager.SpawnedObjects[networkObjectId];
        no.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGrappling)
        {
            isGrappling = false;
            playerScript.movementScript.Enable();
        }
    }

    private void OnGrappleRecharged()
    {
        grappleReady = true;
    }

    private void SetLockonIconPosition()
    {
        if (lockedTarget)
        {
            Vector2 end = Camera.main.WorldToScreenPoint(lockedTarget.position);

            if (lerpTime < 1)
            {
                lerpTime = Mathf.Clamp01(lerpTime + Time.deltaTime * lerpSpeed);
                grappleLockIcon.transform.position = Vector2.Lerp(lockLerpStart, end, lerpCurve.Evaluate(lerpTime));
            }
            else
            {
                grappleLockIcon.transform.position = end;
            }
        }
    }

    private void CheckGrappleTargets()
    {
        if (GameManager.Instance.renderedGrappleTargets.Count > 0)
        {
            // sort by visible
            StaticUtilities.SortByVisible(ref GameManager.Instance.renderedGrappleTargets, "Interactable");

            // sort by distance
            StaticUtilities.SortByDistanceToScreenCenter(ref GameManager.Instance.renderedGrappleTargets);

            // is it the same?
            if (lockedTarget == GameManager.Instance.renderedGrappleTargets[0]) return;
            // are we switching?
            else if (lockedTarget)
            {
                lerpTime = 0f;
                lockLerpStart = Camera.main.WorldToScreenPoint(lockedTarget.position);
            }

            // if its the first / change
            lockedTarget = GameManager.Instance.renderedGrappleTargets[0];
            grappleLockIcon.enabled = true;
        }
        else
        {
            grappleLockIcon.enabled = false;
        }
    }
}
