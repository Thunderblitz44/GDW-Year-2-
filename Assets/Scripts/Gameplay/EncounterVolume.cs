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
        if (startEncounter && (startTimer+=Time.deltaTime) > StaticUtilities.encounterStartDelay)
        {
            startEncounter = false;
            LevelManager.Instance.StartEncounter(bc.bounds, Id);
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
}
