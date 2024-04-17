using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionPortalDetect : MonoBehaviour
{
    public OcclusionPortal DarkSoulsVeil;


    private void OnTriggerEnter(Collider Player)
    {
        DarkSoulsVeil.open = false;
    }
    private void OnTriggerExit(Collider Player)
    {
        DarkSoulsVeil.open = true;
    }

}
