using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Checkpoint))]
public class EncounterVolume : MonoBehaviour
{
    [SerializeField] List<GameObject> barriers;
    BoxCollider bc;
    bool startEncounter = false;
    float startTimer;
    Checkpoint cp;
    protected int totalSpawned = 0;


    public Bounds EncounterBounds { get; private set; }
    public int Id { get; set; }

    private void Awake()
    {
        bc = GetComponent<BoxCollider>();
        cp = GetComponent<Checkpoint>();
        cp.SetOnTriggerEnter = false;
    }

    private void Update()
    {
        if (startEncounter && (startTimer += Time.deltaTime) > StaticUtilities.encounterStartDelay)
        {
            startEncounter = false;
            LevelManager.Instance.SetEncounter(Id);
            StartEncounter();
            foreach (var barrier in barriers)
            {
                barrier.SetActive(true);
            }

            bc.enabled = false;
        }
    }

    private void OnEnable()
    {
        startTimer = 0;
        bc.enabled = true;
        EncounterBounds = bc.bounds;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        startEncounter = true;
    }

    public void EndEncounter()
    {
        foreach (var barrier in barriers)
        {
            barrier.SetActive(false);
        }

        LevelManager.Instance.SetCheckpoint(cp.Id);
        Disable();
    }

    public void Disable()
    {
        if (!bc) bc = GetComponent<BoxCollider>();
        bc.enabled = false;
        enabled = false;
    }

    void StartEncounter()
    {
        StartCoroutine(EncounterRoutine());
    }

    protected virtual IEnumerator EncounterRoutine()
    {
        Transform playerTransform;
        // start
        for (int i = 0; i < 5; i++)
        {
            playerTransform = LevelManager.PlayerTransform;
            // pick random spot in the volume
            Vector3 spawnPoint = LevelManager.GetRandomEnemySpawnPoint(EncounterBounds);
            Vector3 playerPos = Vector3.right * playerTransform.position.x + Vector3.forward * playerTransform.position.z + Vector3.up * spawnPoint.y;
            Quaternion spawnRotation = Quaternion.LookRotation(spawnPoint - playerPos, Vector3.up);
            LevelManager.spawnedEnemies.Add(Instantiate(LevelManager.Instance.LevelEnemyList[Random.Range(0, LevelManager.Instance.LevelEnemyList.Count)], spawnPoint, spawnRotation).GetComponent<DamageableEntity>());

            yield return new WaitForSeconds(0.5f);
        }
        // encounter loop
        while (LevelManager.spawnedEnemies.Count > 0)
        {
            for (int i = 0; i < LevelManager.spawnedEnemies.Count; i++)
            {
                if (LevelManager.spawnedEnemies[i] == null) LevelManager.spawnedEnemies.RemoveAt(i);
            }
            yield return new WaitForSeconds(0.1f);
        }

        // end
        EndEncounter();
    }

    protected void SpawnEnemy(int i = -1)
    {
        if (i == -1) i = Random.Range(0, LevelManager.Instance.LevelEnemyList.Count);
        totalSpawned++;
        Transform playerTransform = LevelManager.PlayerTransform;
        Vector3 spawnPoint = LevelManager.GetRandomEnemySpawnPoint(EncounterBounds);
        Vector3 playerPos = Vector3.right * playerTransform.position.x + Vector3.forward * playerTransform.position.z + Vector3.up * spawnPoint.y;
        Quaternion spawnRotation = Quaternion.LookRotation(spawnPoint - playerPos, Vector3.up);
        LevelManager.spawnedEnemies.Add(Instantiate(LevelManager.Instance.LevelEnemyList[i], spawnPoint, spawnRotation).GetComponent<DamageableEntity>());
    }

    protected void SpawnEnemy(Vector3 pos, int i = -1)
    {
        if (i == -1) i = Random.Range(0, LevelManager.Instance.LevelEnemyList.Count);
        totalSpawned++;
        Transform playerTransform = LevelManager.PlayerTransform;
        Vector3 playerPos = Vector3.right * playerTransform.position.x + Vector3.forward * playerTransform.position.z + Vector3.up * pos.y;
        Quaternion spawnRotation = Quaternion.LookRotation(pos - playerPos, Vector3.up);
        LevelManager.spawnedEnemies.Add(Instantiate(LevelManager.Instance.LevelEnemyList[i], pos, spawnRotation).GetComponent<DamageableEntity>());
    }

    protected void CheckRemaining()
    {
        for (int n = 0; n < LevelManager.spawnedEnemies.Count; n++)
        {
            if (LevelManager.spawnedEnemies[n] == null) LevelManager.spawnedEnemies.RemoveAt(n);
        }
    }
}