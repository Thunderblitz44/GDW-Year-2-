using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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
    Image grappleLockIcon;
    bool isGrappling = false;
    bool grappleReady = true;
    GrappleUI grappleUI;
    bool chargeGrapple = false;
    float grappleChargeTime;

    [SerializeField] float lerpSpeed = 10f;
    [SerializeField] AnimationCurve lerpCurve = AnimationCurve.EaseInOut(0,0,1,1);
    float lerpTime;

    float targetsCheckDelay = 0.1f;
    float timer = 0f;

    Transform lockedTarget;
    Vector2 lockLerpStart;

    Player playerScript;
    Rigidbody rb;

    void Start()
    {
        playerScript = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();

        grappleUI = Instantiate(grappleMeterPrefab, GameManager.Instance.canvas).GetComponent<GrappleUI>();
        grappleUI.onGrappleRecharged += OnGrappleRecharged;
        grappleLockIcon = playerScript.hud?.grappleLockIcon;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (chargeGrapple)
        {
            grappleChargeTime += Time.deltaTime;
        }



        // slerp lock icon
        if (lockedTarget)
        {
            Vector2 end = Camera.main.WorldToScreenPoint(lockedTarget.position);
            grappleLockIcon.transform.position = Vector2.Lerp(lockLerpStart, end, lerpCurve.Evaluate(lerpTime+=Time.deltaTime*lerpSpeed));
        }



        timer += Time.deltaTime;
        if (timer < targetsCheckDelay) return;
        timer = 0f;

        if (GameManager.Instance.renderedGrappleTargets.Count > 0)
        {

            StaticUtilities.SortByDistanceToScreenCenter(GameManager.Instance.renderedGrappleTargets);

            // sort by priority

            // lock on to visible
            /* foreach (var item in GameSettings.instance.renderedGrappleTargets)
             {
                 RaycastHit hit;
                 if (Physics.Raycast(Camera.main.transform.position, (Camera.main.transform.position-item.position).normalized, out hit))
                 {
                     if (hit.transform.gameObject.layer != whatIsGrapplable)
                     {
                         Debug.Log("theres something in the way");
                     }
                     else
                     {
                         Debug.Log("all good");
                         break;
                     }
                 }
             }*/

            if (lockedTarget && lockedTarget != GameManager.Instance.renderedGrappleTargets[0])
            {
                lerpTime = 0f;
                lockLerpStart = Camera.main.WorldToScreenPoint(lockedTarget.position);
            }
            lockedTarget = GameManager.Instance.renderedGrappleTargets[0];

            if (!lockedTarget) return;

            Vector3 point = Camera.main.WorldToScreenPoint(lockedTarget.position);
            point.z = 0;

            grappleLockIcon.enabled = true;

        }
        else
        {
            grappleLockIcon.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGrappling)
        {
            isGrappling = false;
            playerScript.movementScript.Enable();
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
    }
    
    public override void Part2()
    {
    }

    

    private void OnGrappleRecharged()
    {
        grappleReady = true;
    }

}
