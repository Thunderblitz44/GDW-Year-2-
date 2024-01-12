using System;
using System.Collections.Generic;
using UnityEngine;

public class EncounterVolume : MonoBehaviour
{
    [SerializeField] List<GameObject> barriers;
    public Action<Bounds, int> onEncounterStarted;
    BoxCollider bc;
    bool startEncounter = false;
    float startTimer;
    [HideInInspector] public int id;

    private void Awake()
    {
        bc = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (startEncounter && (startTimer+=Time.deltaTime) > StaticUtilities.encounterStartDelay)
        {
            startEncounter = false;
            onEncounterStarted?.Invoke(bc.bounds, id);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        startEncounter = true;
    }

    public void EndEncounter()
    {
        foreach (var barrier in barriers)
        {
            barrier.SetActive(false);
        }

        gameObject.SetActive(false);
    }
}
