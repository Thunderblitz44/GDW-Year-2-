using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
public class EnvironmentalParticles : MonoBehaviour
{
    private float sceneValue;
    public VisualEffect EnvironmentalVFX;

    void Start()
    {
      
        string sceneName = SceneManager.GetActiveScene().name;

  
        switch (sceneName)
        {
            case "Level1":
                sceneValue = 1f;
                break;
            case "Level2":
                sceneValue = 2f;
                break;
            case "Level3":
                sceneValue = 3f;
                break;
            case "FinalBoss":
                sceneValue = 4f;
                break;
            default:
                sceneValue = 0f; 
                break;
        }

   
        //Debug.Log("Current Scene Value: " + sceneValue);
        Invoke("SetEnviVFX", 2f);
   
    }


    void SetEnviVFX()
    {
        
        switch (sceneValue)
        {
            case 1f:
                EnvironmentalVFX.SendEvent("Level1");
                break;
            case 2f:
                EnvironmentalVFX.SendEvent("Level2");
                break;
            case 3f:
                EnvironmentalVFX.SendEvent("Level3");
                break;
            case 4f:
                EnvironmentalVFX.SendEvent("FinalBoss");
                break;
            default:
              
                break;
        }
    }
}
