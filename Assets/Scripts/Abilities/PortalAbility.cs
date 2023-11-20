using UnityEngine;

public class PortalAbility : Ability
{
    [SerializeField] float minTeleportDist = 5f;
    [SerializeField] float maxTeleportDist = 30f;
    [SerializeField] float portalChargeSpeed = 2f;
    [SerializeField] int maxUses = 2;
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject previewPortalPrefab;
    Transform previewPortal;
    Vector3 portalAPos;
    Vector3 portalBPos;
    Quaternion portalRotation;
    bool chargePortals;
    float previewDist;
    bool usingPortalAbility;
    float portalChargeTime;
    bool hitWall;

    Vector3 lastPortalPos;

    Player playerScript;
    Transform body;

    void Start()
    {
        playerScript = GetComponent<Player>();
        body = playerScript.movementScript.GetBody();
    }

    void Update()
    {
        if (chargePortals)
        {
            SetPortalDistance();
            previewPortal.rotation = body.rotation;
        }
    }


    void SetPortalDistance()
    {
        // increase charge
        if (!hitWall) portalChargeTime += Time.deltaTime * portalChargeSpeed;

        // set the new distance
        previewDist = Mathf.Clamp(portalChargeTime + minTeleportDist, minTeleportDist, maxTeleportDist);

        Vector3 aimPoint = transform.position + StaticUtilities.GetCameraLook() * previewDist;

        Vector3 newpos;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, StaticUtilities.GetCameraDir(), out hit, previewDist))
        {
            newpos = new Vector3(hit.point.x, previewPortal.position.y, hit.point.z) - StaticUtilities.GetCameraDir();
            // set charge for that distance
            if (!hitWall) portalChargeTime = hit.distance - minTeleportDist; 
            hitWall = true;
        }
        else
        {
            hitWall = false;
            newpos = transform.position + StaticUtilities.GetCameraDir() * previewDist;
        }

        if (Physics.Raycast(aimPoint, Vector3.down, out hit, previewDist))
        {
            newpos.y = hit.point.y + 1;
        }
        else
        {
            newpos.y = transform.position.y;
        }

        previewPortal.position = newpos;
        //lastPortalPos = newpos;
    }

    public override void Part1()
    {
        if (usingPortalAbility) return;

        usingPortalAbility = true;
        chargePortals = true;

        previewPortal = Instantiate(previewPortalPrefab, transform.position + StaticUtilities.GetCameraLook() * minTeleportDist, body.rotation).transform;
    }

    public override void Part2()
    {
        chargePortals = false;
        portalChargeTime = 0f;

        portalRotation = previewPortal.rotation;
        portalAPos = body.position + body.forward * 1.5f;
        portalBPos = previewPortal.position;

        Destroy(previewPortal.gameObject);

        // set up the portals
        Transform entryPortal = Instantiate(portalPrefab, portalAPos, portalRotation).transform;
        entryPortal.forward = -entryPortal.forward;
        Transform exitPortal = Instantiate(portalPrefab, portalBPos, portalRotation).transform;
        var f = entryPortal.GetComponent<Portal>();
        var s = exitPortal.GetComponent<Portal>();
        f.Init(maxUses, s);
        s.Init(maxUses, f);

        usingPortalAbility = false;
    }
}
