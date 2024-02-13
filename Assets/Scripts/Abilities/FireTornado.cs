using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTornado : MonoBehaviour
{
    List<Rigidbody> bodies = new();
    [HideInInspector] public Vector2 force = Vector2.one * 10f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb;
        if (other.gameObject.TryGetComponent(out rb))
        {
            bodies.Add(rb);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Fire Tornado Placeholder");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb;
        if (other.gameObject.TryGetComponent(out rb))
        {
            bodies.Remove(rb);
        }
    }

    private void Update()
    {
        foreach (var body in bodies)
        {
            body.AddForce((body.transform.position - transform.position).normalized * force.x + Vector3.up * force.y, ForceMode.Force);
        }   
    }
}
