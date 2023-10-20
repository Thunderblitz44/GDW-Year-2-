using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeScript : MonoBehaviour
{
    public GameObject overlayMenu;
    private PlayerMenu playerMenu;
    private void Awake()
    {
        playerMenu = overlayMenu.GetComponent<PlayerMenu>();
        playerMenu.InstantiateOverlay();
    }

    // Start is called before the first frame update
    public void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
