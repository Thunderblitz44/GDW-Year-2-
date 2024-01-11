using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class legController : MonoBehaviour
{
    public golemIkScript[] scriptsToControl; 
    private int activeScriptIndex = 0; 

    void Start()
    {
       
        for (int i = 0; i < scriptsToControl.Length; i++)
        {
            if (i == 0)
            {
                scriptsToControl[i].footDelay = true; 
                activeScriptIndex = 0;
            }
            else
            {
                scriptsToControl[i].footDelay = false; 
            }
        }
    }
    public void SetActiveScript()
    {
    
            scriptsToControl[activeScriptIndex].footDelay = false;

      
            activeScriptIndex = (activeScriptIndex + 1) % scriptsToControl.Length;
            scriptsToControl[activeScriptIndex].footDelay = true;
    }
    void Update()
    {
       
    }
}

