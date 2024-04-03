using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter battleMusicEmitter;
    [SerializeField] private StudioEventEmitter bossMusicEmitter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginBattleMusic()
    {
        if (battleMusicEmitter != null)
        {
            battleMusicEmitter.SetParameter("EncounterMusic", 1f);
          
        }
        else
        {
            Debug.LogError("Battle music emitter reference is null.");
        }
    }

    public void EndBattleMusic()
    {
        if (battleMusicEmitter != null)
        {
            battleMusicEmitter.SetParameter("EncounterMusic", 0f);
          
        }
        else
        {
            Debug.LogError("Battle music emitter reference is null.");
        }
    }

    public void BeginBossMusic()
    {
      
        if (bossMusicEmitter != null)
        {
          
            bossMusicEmitter.SetParameter("BossMusic", 1f);
            if (battleMusicEmitter != null)
            {
                battleMusicEmitter.Stop();
            }
            else
            {
                Debug.LogError("Battle music emitter reference is null.");
            }
        }
        else
        {
            Debug.LogError("Boss music emitter reference is null.");
        }
    }

    public void EndBossMusic()
    {
        if (bossMusicEmitter != null)
        {
            Debug.Log("FRICK");
            bossMusicEmitter.SetParameter("BossMusic", 0f);
            if (battleMusicEmitter != null)
            {
                battleMusicEmitter.Play();
            }
            else
            {
                Debug.LogError("Battle music emitter reference is null.");
            }
        }
        else
        {
            Debug.LogError("Boss music emitter reference is null.");
        }
    }


    public void ChangeBossMusic()
    {
        bossMusicEmitter.SetParameter("Stage2", 1f);
    }
}
