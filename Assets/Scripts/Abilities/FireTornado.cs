using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FireTornado : MonoBehaviour
{
    List<Rigidbody> bodies = new();
    [HideInInspector] public Vector2 force = Vector2.zero;
    [HideInInspector] public float burnTime = 0;
    [HideInInspector] public float damage = 0;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb;
        if (other.gameObject.TryGetComponent(out rb))
        {
            rb.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            bodies.Add(rb);
            StaticUtilities.TryToDamageOverTime(other.gameObject, damage, burnTime);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Fire Tornado Placeholder");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb;
        if (other.gameObject.TryGetComponent(out rb))
        {
            rb.gameObject.GetComponent<NavMeshAgent>().enabled = true;
            bodies.Remove(rb);
        }
    }

    private void Update()
    {
        foreach (var body in bodies)
        {
            body.AddForce(Vector3.up * force.y, ForceMode.Force);
        }   
    }
}
