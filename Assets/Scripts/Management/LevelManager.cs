using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public static int Id { get; private set; }
    public static Transform PlayerTransform { get; private set; }
    public Player PlayerScript { get; private set; }
    public static bool isGamePaused = false;
    public static bool isPlayerDead = false;


    public NavMeshSurface NavMesh { get; private set; }
    [SerializeField] List<EncounterVolume> encounterVolumes;
    [SerializeField] List<Checkpoint> checkpoints = new();
    [SerializeField] List<GameObject> enemies;
    [SerializeField] GameObject boss;
    readonly List<DamageableEntity> spawnedEnemies = new();

    public Transform WorldCanvas { get; private set; }
    public Transform Canvas { get; private set; }

    public Checkpoint CurrentCheckpoint { get; private set; }
    EncounterVolume currentEncounter;
    Transitioner transitioner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("There were 2 LevelManager scripts!");
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
        NavMesh = FindFirstObjectByType<NavMeshSurface>();
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

    public void StartEncounter(Bounds volumeBounds, int id)
    {
        currentEncounter = encounterVolumes[id];
        if (id == encounterVolumes.Count - 1) boss.GetComponent<IBossCommands>().Introduce();
        else StartCoroutine(EncounterRoutine(volumeBounds));
    }

    IEnumerator EncounterRoutine(Bounds volumeBounds)
    {
        // start
        for (int i = 0; i < 5; i++)
        {
            // pick random spot in the volume
            Vector3 spawnPoint = GetRandomEnemySpawnPoint(volumeBounds);
            Vector3 playerPos = Vector3.right * PlayerTransform.position.x + Vector3.forward * PlayerTransform.position.z + Vector3.up * spawnPoint.y;
            Quaternion spawnRotation = PlayerTransform ? Quaternion.LookRotation(spawnPoint - playerPos, Vector3.up) : Quaternion.identity;
            spawnedEnemies.Add(Instantiate(enemies[0], spawnPoint, spawnRotation).GetComponent<DamageableEntity>());

            yield return new WaitForSeconds(0.5f);
        }

        // encounter loop
        while (spawnedEnemies.Count > 0)
        {
            for(int i = 0;i < spawnedEnemies.Count;i++) 
            {
                if (spawnedEnemies[i] == null) spawnedEnemies.RemoveAt(i);
            }
            yield return new WaitForSeconds(0.1f);
        }

        // end
        currentEncounter.EndEncounter();
    }

    Vector3 GetRandomEnemySpawnPoint(Bounds volumeBounds)
    {
        int itterations = 0;
        Start:
        Vector3 spawnPoint = Vector3.right * Random.Range(volumeBounds.min.x, volumeBounds.max.x) + Vector3.up * volumeBounds.max.y + Vector3.forward * Random.Range(volumeBounds.min.z, volumeBounds.max.z);
        RaycastHit hit;
        if (Physics.Raycast(spawnPoint, Vector3.down, out hit, 100f, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Distance(hit.point, PlayerTransform.position) < 3) goto Start;

            foreach (var enemy in spawnedEnemies)
            {
                if (Vector3.Distance(enemy.transform.position, hit.point) < 2) goto Start;
            }

            return hit.point + Vector3.up;
        }
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
        if (!CurrentCheckpoint) return;
        PlayerScript.MovementScript.DisableLocomotion();
        //PlayerScript.PausePlayer();
        transitioner.FadeToBlack(delay);
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(StaticUtilities.CURRENT_LEVEL, Id);
        PlayerPrefs.SetInt(StaticUtilities.CURRENT_CHECKPOINT, CurrentCheckpoint.Id);
        PlayerPrefs.SetFloat(StaticUtilities.CURRENT_PLAYER_HEALTH, PlayerTransform.GetComponent<HealthComponent>().health);
        if (currentEncounter) PlayerPrefs.SetInt(StaticUtilities.LAST_ENCOUNTER, currentEncounter.Id);
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
            if (le >= 0) currentEncounter = encounterVolumes[le];

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
            currentEncounter = encounterVolumes.Last();

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
        if (isPlayerDead) SceneManager.LoadScene(Id);
        else
        {
            transitioner.FadeToClear(0.5f);
            CurrentCheckpoint.Teleport(PlayerTransform);
        }
    }

    void ScreenIsClear()
    {
        PlayerScript.MovementScript.EnableLocomotion();
    }
}
