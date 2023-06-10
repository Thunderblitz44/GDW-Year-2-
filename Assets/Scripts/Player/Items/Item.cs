using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] bool interactable = true;
    [SerializeField] bool inspectable = true;

    [Header("Properties")]
    [SerializeField] string description;
    [SerializeField] float weight;
    [SerializeField] float value;
    Dictionary<string, string> data = new Dictionary<string, string>();

    SphereCollider sc;
    Rigidbody rb;
    BoxCollider bc;

    private void Awake()
    {
        sc = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        data.Add("Description:", description);
        data.Add("Weight:", $"{weight}kg");
        data.Add("Value:", $"{value}$");
    }

    public void Pickup()
    {
        sc.enabled = false;
        rb.isKinematic = true;
        bc.enabled = false;
    }

    public void Drop()
    {
        sc.enabled = true;
        rb.isKinematic = false;
        bc.enabled = true;
    }


    public bool IsInteractable() => interactable;
    public bool IsInspecable() => inspectable;
    public Dictionary<string, string> GetProperties() => data;

}
