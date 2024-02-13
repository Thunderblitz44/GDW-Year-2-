using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LesserSpirit : DamageableEntity
{
    [Header("Spirit Stuff")]
    [SerializeField] float hoverVerticalDeviation = 0.5f;
    [SerializeField] float hoverDeviationSpeed = 1f;
    Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //transform.position += Vector3.up * (hoverVerticalDeviation * Mathf.Sin(Time.deltaTime * hoverDeviationSpeed));
    }
}
