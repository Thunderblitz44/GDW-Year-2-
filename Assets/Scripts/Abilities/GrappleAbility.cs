using Unity.Netcode;
using UnityEngine;

public class GrappleAbility : Ability
{
    [SerializeField] float grappleDistance = 50f;
    [SerializeField] float lightWeightMaxMass = 1f;
    [SerializeField] float overshoot = 5f;
    [SerializeField] float playerLandingSpace = 1f;
    [SerializeField] float meetInTheMiddleSpacing = 0.5f;
    [SerializeField] float itemLandingSpace = 1f;
    [SerializeField] float grappleCooldown = 3f;
    [SerializeField] LayerMask whatIsGrapplable;
    [SerializeField] GameObject grappleMeterPrefab;
    bool isGrappling = false;
    bool grappleReady = true;
    GrappleUI grappleUI;
    bool chargeGrapple = false;
    float grappleChargeTime;

    Player playerScript;
    Rigidbody rb;

    void Start()
    {
        playerScript = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();

        grappleUI = Instantiate(grappleMeterPrefab, GameSettings.instance.GetCanvas()).GetComponent<GrappleUI>();
        grappleUI.onGrappleRecharged += OnGrappleRecharged;

    }

    void Update()
    {
        if (!IsOwner) return;

        if (chargeGrapple)
        {
            grappleChargeTime += Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGrappling)
        {
            isGrappling = false;
            playerScript.GetMovementScript().Enable();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void LaunchTargetServerRpc(ulong networkObjectId, Vector3 force)
    {
        NetworkObject no = NetworkManager.SpawnManager.SpawnedObjects[networkObjectId];
        no.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }


    public override void Part1()
    {
        if (isGrappling || !grappleReady) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grappleDistance, whatIsGrapplable))
        {
            grappleUI.SpendPoint();

            Vector3 launchForce = Vector3.zero;
            Transform target = hit.transform;
            Vector3 dir = (target.position - transform.position).normalized;

            // process the grapple target
            if (!hit.rigidbody)
            {
                // grapple point - we go to it
                float dist = Vector3.Distance(transform.position, target.position) - playerLandingSpace;
                Vector3 playerEndPoint = transform.position + dir * dist;
                launchForce = StaticUtilities.CalculateLaunchVelocity(transform.position, playerEndPoint, overshoot);
            }
            else if (hit.rigidbody.mass > lightWeightMaxMass)
            {
                // something heavy, meet in the middle
                float dist = Vector3.Distance(transform.position, target.position) / 2 - meetInTheMiddleSpacing;
                Vector3 playerEndPoint = transform.position + dir * dist;
                Vector3 targetEndPoint = target.position - dir * dist;

                launchForce = StaticUtilities.CalculateLaunchVelocity(transform.position, playerEndPoint, overshoot);

                // tell the server to launch the target
                LaunchTargetServerRpc(target.GetComponent<NetworkObject>().NetworkObjectId, StaticUtilities.CalculateLaunchVelocity(target.position, targetEndPoint, overshoot) * hit.rigidbody.mass);
            }
            else
            {
                // light object, it comes to us
                float dist = Vector3.Distance(transform.position, target.position) - itemLandingSpace;
                Vector3 targetEndPoint = target.position - dir * dist;

                // tell the server to launch the target
                LaunchTargetServerRpc(target.GetComponent<NetworkObject>().NetworkObjectId, StaticUtilities.CalculateLaunchVelocity(target.position, targetEndPoint, overshoot) * hit.rigidbody.mass);
                goto cooldown;
            }

            isGrappling = true;
            playerScript.GetMovementScript().Disable();
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(launchForce * rb.mass, ForceMode.Impulse);

        cooldown:
            grappleReady = false;
            grappleUI.RechargePoint(grappleCooldown);
        }
    }
    
    public override void Part2()
    {
    }

    

    private void OnGrappleRecharged()
    {
        grappleReady = true;
    }

}
