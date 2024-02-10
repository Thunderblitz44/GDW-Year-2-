using System;
using System.Collections;
using UnityEngine;

public class Level1Encounter3 : EncounterVolume
{
    [Header("Spirit")]
    [SerializeField] Transform spirit;
    [SerializeField] Vector3 startScale = Vector3.one * 0.25f;
    [SerializeField] Vector3 endScale = Vector3.one * 0.25f;
    [SerializeField] Transform gotoTransform;

    [Header("Challenge")]
    [SerializeField] float surviveTime = 30f;
    [SerializeField] GameObject[] torchLights;

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

        // make the spirit grow
        for (float i = 0; i <= surviveTime; i+=Time.deltaTime)
        {
            spirit.localScale = Vector3.Lerp(startScale, endScale, i / surviveTime);

            // spawn enemies

            yield return null;
        }

        // Make the pillar fall over
        for (float i = 0; i <= 1; i += Time.deltaTime * fallSpeed)
        {
            pillar.rotation = Quaternion.Lerp(pillarPoses[0].rotation, pillarPoses[1].rotation, pillarFallCurve.Evaluate(i));
            pillar.position = Vector3.Slerp(pillarPoses[0].position, pillarPoses[1].position, pillarFallCurve.Evaluate(i));
            yield return null;
        }
        pillar.position = pillarPoses[1].position;
        pillar.rotation = pillarPoses[1].rotation;

        //EndEncounter();
    }
}
