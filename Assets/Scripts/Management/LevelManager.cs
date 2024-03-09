using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public static int Id { get; private set; }
    public static Transform PlayerTransform { get; private set; }
    public Player PlayerScript { get; private set; }
    public static bool isGamePaused = false;
    public static bool isGameOver = false;

    [SerializeField] List<EncounterVolume> encounterVolumes;
    [SerializeField] List<Checkpoint> checkpoints = new();
    [SerializeField] List<GameObject> enemies;
    [SerializeField] GameObject boss;
    public static readonly List<DamageableEntity> spawnedEnemies = new();
    public Action onEncounterStart;

    public GameObject Boss { get { return boss; } }
    public List<GameObject> LevelEnemyList { get { return enemies; } }
    public Transform WorldCanvas { get; private set; }
    public Transform Canvas { get; private set; }

    public Checkpoint CurrentCheckpoint { get; private set; }
    public EncounterVolume CurrentEncounter { get; private set; }
    Transitioner transitioner;

    public GameObject floatingTextPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("There were multiple LevelManager scripts!");
            Destroy(this);
            return;
        }
        Instance = this;

        // give encounterVolumes their ids
        for (int i = 0; i < encounterVolumes.Count; i++)
        {
            encounterVolumes[i].Id = i;
        }

        // give checkpoints their ids
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].Id = i;
        }

        Id = SceneManager.GetActiveScene().buildIndex;
        Canvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
        WorldCanvas = GameObject.FindGameObjectWithTag("WorldCanvas").transform;
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerScript = PlayerTransform.GetComponent<Player>();
        transitioner = Canvas.GetChild(Canvas.childCount - 1).GetComponent<Transitioner>();
        if (!transitioner) Debug.LogWarning("The transitioner needs to be the last child of canvas!");
        else
        {
            transitioner.onFadedToBlack += ScreenIsBlack;
            transitioner.onFadedToClear += ScreenIsClear;
        }

        LoadProgress();
    }

    public void SetEncounter(int id)
    {
        CurrentEncounter = encounterVolumes[id];
        onEncounterStart?.Invoke();
    }

    public static Vector3 GetRandomEnemySpawnPoint(Bounds volumeBounds)
    {
        int itterations = 0;
    Start:
        Vector3 spawnPoint = Vector3.right * UnityEngine.Random.Range(volumeBounds.min.x, volumeBounds.max.x) + Vector3.up * volumeBounds.center.y + Vector3.forward * UnityEngine.Random.Range(volumeBounds.min.z, volumeBounds.max.z);
        if (NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, 30f, NavMesh.AllAreas))
        {
            if (Vector3.Distance(hit.position, PlayerTransform.position) < 3) goto Start;

            foreach (var enemy in spawnedEnemies)
            {
                if (!enemy) continue;
                if (Vector3.Distance(enemy.transform.position, hit.position) < 2) goto next;
            }

            return hit.position + Vector3.up * 2f;
        }

    next:
        if (++itterations < 20) goto Start;
        else
        {
            Debug.LogWarning("Couldn't find a spawn point! Returning a zero vector...");
            return Vector3.zero;
        }
    }

    public void SetCheckpoint(int id)
    {
        Debug.Log("Checkpoint set");
        CurrentCheckpoint = checkpoints[id];
        SaveProgress();
    }

    public void Respawn(float delay = 0)
    {
        PlayerScript.FreezeCamera();
        transitioner.FadeToBlack(delay);
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(StaticUtilities.CURRENT_LEVEL, Id);
        PlayerPrefs.SetInt(StaticUtilities.CURRENT_CHECKPOINT, CurrentCheckpoint.Id);
        PlayerPrefs.SetInt(StaticUtilities.CURRENT_PLAYER_HEALTH, PlayerTransform.GetComponent<HealthComponent>().Health);
        if (CurrentEncounter) PlayerPrefs.SetInt(StaticUtilities.LAST_ENCOUNTER, CurrentEncounter.Id);
    }

    void LoadProgress()
    {
        int cl = PlayerPrefs.GetInt(StaticUtilities.CURRENT_LEVEL, 0);
        int cc = PlayerPrefs.GetInt(StaticUtilities.CURRENT_CHECKPOINT, 0);
        int le = PlayerPrefs.GetInt(StaticUtilities.LAST_ENCOUNTER, -1);

        // if this is the first time playing this level
        if (Id == cl)
        {
            CurrentCheckpoint = checkpoints[cc];
            if (le >= 0) CurrentEncounter = encounterVolumes[le];

            // disable old encounters
            foreach (var volume in encounterVolumes)
            {
                if (volume.Id <= le) volume.Disable();
            }

            // disable old checkpoints
            foreach (var checkpoint in checkpoints)
            {
                if (!checkpoint.isActiveAndEnabled) continue;
                if (checkpoint.Id < cc) checkpoint.Disable(); 
            }
        }
        
        // if we already beat this level
        else if (Id < cl)
        {
            CurrentCheckpoint = checkpoints.Last();
            CurrentEncounter = encounterVolumes.Last();

            // disble all encounters
            foreach (var volume in encounterVolumes)
            {
                volume.Disable();
            }

            // disable old checkpoints
            foreach (var checkpoint in checkpoints)
            {
                if (!checkpoint.isActiveAndEnabled) continue;
                if (checkpoint.Id < cc) checkpoint.Disable();
            }
        }

        // if this is a new level
        else if (Id > cl)
        {
            PlayerPrefs.SetInt(StaticUtilities.CURRENT_LEVEL, Id);
            PlayerPrefs.SetInt(StaticUtilities.CURRENT_CHECKPOINT, 0);
            PlayerPrefs.SetInt(StaticUtilities.LAST_ENCOUNTER, -1);

            CurrentCheckpoint = checkpoints[0];
        }

        transitioner.SetToBlack();
        transitioner.FadeToClear();
    }

    void ScreenIsBlack()
    {
        if (isGameOver) SceneManager.LoadScene(Id);
        else
        {
            transitioner.FadeToClear(0.5f);
            if (CurrentCheckpoint) CurrentCheckpoint.Teleport(PlayerTransform);
            PlayerScript.UnFreezeCamera();
        }
    }

    void ScreenIsClear()
    {
        PlayerScript.UnPausePlayer();
    }
}
