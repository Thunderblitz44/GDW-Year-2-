using System.Collections;
using UnityEngine;

public class Level1Encounter3 : EncounterVolume
{
    [Header("Spirit")]
    [SerializeField] Transform spirit;
    [SerializeField] Vector3 startScale = Vector3.one * 0.25f;
    [SerializeField] Vector3 endScale = Vector3.one;
    [SerializeField] Transform gotoTransform;
    [SerializeField] float moveSpeed;

    [Header("Challenge")]
    [SerializeField] float surviveTime = 30f;
    [SerializeField] GameObject[] torchLights;
    [SerializeField] float ignitionDelay = 0.25f;
    [SerializeField] float enemySpawnDelay = 1f;

    [Header("Pillar")]
    [SerializeField] Transform pillar;
    [SerializeField] Transform[] pillarPoses;
    [SerializeField] float fallSpeed;
    [SerializeField] AnimationCurve pillarFallCurve = AnimationCurve.Linear(0, 0, 1, 1);

    protected override void Awake()
    {
        base.Awake();
        if (bc.enabled)
        {
            pillar.position = pillarPoses[0].position;
            pillar.rotation = pillarPoses[0].rotation;
        }

        spirit.localScale = startScale;
    }

    protected override void Update()
    {
        base.Update();

        // move the spirit
    }

    protected override IEnumerator EncounterRoutine()
    {
        yield return new WaitForSeconds(1f);

        // light the torches
        foreach (var torch in torchLights)
        {
            torch.SetActive(true);
            yield return new WaitForSeconds(ignitionDelay);
        }

        // make the spirit grow
        for (float i = 0, delay = surviveTime / torchLights.Length; i <= surviveTime + 1; i+=Time.deltaTime)
        {
            // grow the spirit
            spirit.localScale = Vector3.Lerp(startScale, endScale, i / surviveTime);

            // extinguish torches
            int n = Mathf.FloorToInt(i / delay);
            if (n > 0 && torchLights[n-1].activeSelf) torchLights[n-1].SetActive(false);

            // spawn enemies
            if (Mathf.FloorToInt(i / enemySpawnDelay) > totalSpawned) SpawnEnemy();

            yield return null;
        }

        // wait until clear
        while (LevelManager.spawnedEnemies.Count > 0)
        {
            for (int i = 0; i < LevelManager.spawnedEnemies.Count; i++)
            {
                if (LevelManager.spawnedEnemies[i] == null) LevelManager.spawnedEnemies.RemoveAt(i);
            }
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(1f);

        // move spirit to pillar
        Vector3 startPos = spirit.position;
        AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        for (float i = 0; i <= 1; i += Time.deltaTime * moveSpeed)
        {
            spirit.position = Vector3.Lerp(startPos, gotoTransform.position, moveCurve.Evaluate(i));
            yield return null;
        }

        yield return new WaitForSeconds(1f);


        // Make the pillar fall over
        for (float i = 0; i <= 1; i += Time.deltaTime * fallSpeed)
        {
            pillar.rotation = Quaternion.Lerp(pillarPoses[0].rotation, pillarPoses[1].rotation, pillarFallCurve.Evaluate(i));
            pillar.position = Vector3.Slerp(pillarPoses[0].position, pillarPoses[1].position, pillarFallCurve.Evaluate(i));
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Pillar Collapse", gameObject);
            yield return null;
        }
        pillar.position = pillarPoses[1].position;
        pillar.rotation = pillarPoses[1].rotation;

        EndEncounter();
    }
}
