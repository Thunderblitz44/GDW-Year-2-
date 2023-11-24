using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elana : Player
{
    [Space(10),Header("ABILITIES"),Space(10)]
    [Header("Portal")]
    [SerializeField] float portalRange = 30f;
    [SerializeField] float portalChargeSpeed = 2f;
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject previewPortalPrefab;
    [SerializeField] LayerMask portalPlacableSurfaces;
    int maxUses = 1;
    Transform previewPortal;
    Vector3 portalAPos;
    Vector3 portalBPos;
    Quaternion portalRotation;
    bool usingPortalAbility;






    Transform body;

    internal override void Start()
    {
        base.Start();
        body = movementScript.GetBody();
    }

    void FixedUpdate()
    {
        if (usingPortalAbility)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, StaticUtilities.GetCameraLook(), out hit, portalRange, portalPlacableSurfaces, QueryTriggerInteraction.Ignore))
            {
                previewPortal.position = hit.point + Vector3.up;
                previewPortal.rotation = body.rotation;
            }
        }
    }

    public override void SetupInputEvents()
    {
        base.SetupInputEvents();
        // Elana's Abilities


        // BASIC
        actions.General.Attack.performed += ctx =>
        {
            GameObject.Find("TestDummy").GetComponent<IDamageable>().ApplyDamage(1f, DamageTypes.physical);
        };
        actions.CameraControl.Aim.performed += ctx =>
        {
            GameObject.Find("TestDummy").GetComponent<IDamageable>().ApplyDamage(-1f, DamageTypes.magic);
        };


        // Abilities

        // PORTAL ABILITY
        actions.Abilities.First.started += ctx => 
        {

            usingPortalAbility = true;
            previewPortal = Instantiate(previewPortalPrefab).transform;
        };
        actions.Abilities.First.canceled += ctx => 
        {
            usingPortalAbility = false;

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
        };


        actions.Abilities.Second.started += ctx => { };
        actions.Abilities.Second.canceled += ctx => { };

        actions.Abilities.Third.started += ctx => { };
        actions.Abilities.Third.canceled += ctx => { };

        actions.Abilities.Fourth.started += ctx => { };
        actions.Abilities.Fourth.canceled += ctx => { };

        actions.Abilities.Enable();
    }
}
