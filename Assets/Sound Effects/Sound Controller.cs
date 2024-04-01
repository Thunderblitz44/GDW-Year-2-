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
        battleMusicEmitter.SetParameter("EncounterMusic", 1f);
    }

    public void EndBattleMusic()
    {
        battleMusicEmitter.SetParameter("EncounterMusic", 0f);
    }

    public void BeginBossMusic()
    {
        bossMusicEmitter.SetParameter("BossMusic", 1f);
        battleMusicEmitter.Stop();
    }

    public void EndBossMusic()
    {
        bossMusicEmitter.SetParameter("BossMusic", 0f);
        battleMusicEmitter.Play();
    }
    
}
