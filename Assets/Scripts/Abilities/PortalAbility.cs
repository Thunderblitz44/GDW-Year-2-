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
    bool hitGround;
    Vector3 previewPortalNewPos;
    Vector3 aimPoint;

    Vector3 lastPortalPos;

    Player playerScript;
    Transform body;

    float slowUpdateTime;
    float slowUpdateInterval = 0.1f;

    void Start()
    {
        playerScript = GetComponent<Player>();
        body = playerScript.movementScript.GetBody();
    }

    void Update()
    {
        if (!chargePortals) return;
        MovePreviewPortal();
        SlowUpdate();
    }

    void SlowUpdate()
    {
        slowUpdateTime += Time.deltaTime;
        if (slowUpdateTime < slowUpdateInterval) return;
        slowUpdateTime = 0;

        CheckPortalPlacement();
    }

    void MovePreviewPortal()
    {
        // increase charge
        if (!hitWall) portalChargeTime += Time.deltaTime * portalChargeSpeed;

        aimPoint = transform.position + StaticUtilities.GetCameraDir() * previewDist;

        // set the new distance
        previewDist = Mathf.Clamp(portalChargeTime + minTeleportDist, minTeleportDist, maxTeleportDist);

        previewPortal.position = aimPoint;
        previewPortal.rotation = body.rotation;
    }

    void CheckPortalPlacement()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, StaticUtilities.GetCameraLook(), out hit, previewDist))
        {
            previewPortalNewPos = new Vector3(hit.point.x, previewPortal.position.y, hit.point.z) - StaticUtilities.GetCameraDir();
            // set charge for that distance
            if (!hitWall) portalChargeTime = hit.distance - minTeleportDist; 
            hitWall = true;
        }
        else
        {
            hitWall = false;
        }

        if (Physics.Raycast(aimPoint, Vector3.down, out hit, previewDist))
        {
            previewPortalNewPos.y = hit.point.y + 1;
            hitGround = true;
        }
        else
        {
            previewPortalNewPos.y = transform.position.y;
            hitGround = false;
        }
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
