using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DashAbility", menuName = "Abilities/Dash")]
public class DashAbilityScriptableObject : ScriptableObject, IAbility
{
    // parameters
    public float distance;
    public float force;
    public float cooldown;

    int i;
    Rigidbody rb;

    public void UseAbility(object player)
    {
        Player playerScript = (Player)player;
        if (!playerScript) return;

        rb = playerScript.GetMovementScript().GetRigidbody();

        rb.AddForce(rb.transform.forward * force, ForceMode.Impulse);
    }

    IEnumerator DashPolisher()
    {
        yield return null;
    }
}
