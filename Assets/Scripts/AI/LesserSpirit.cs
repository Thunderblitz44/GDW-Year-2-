using UnityEngine;

public class LesserSpirit : DamageableEntity
{
    [Header("Spirit Stuff")]
    [SerializeField] float verticalAmplitude = 0.2f;
    [SerializeField] float verticalFrequency = 0.8f;
    [SerializeField] float LeftRightAmplitude = 0.08f;
    [SerializeField] float LeftRightFrequency = 1.6f;
    [SerializeField] float BackFrontAmplitude = 0.02f;
    [SerializeField] float BackFrontFrequency = 0.4f;
    Vector3 startPos;
    bool pauseHover;

    protected override void Awake()
    {
        base.Awake();
        startPos = transform.position;
    }

    private void Update()
    {
        if (pauseHover) return;
        transform.position = startPos + StaticUtilities.BuildVector(Mathf.Sin(Time.timeSinceLevelLoad * LeftRightFrequency) * LeftRightAmplitude,
            Mathf.Sin(Time.timeSinceLevelLoad * verticalFrequency) * verticalAmplitude,
            Mathf.Sin(Time.timeSinceLevelLoad * BackFrontFrequency) * BackFrontAmplitude);
    }

    public void PauseHover()
    {
        pauseHover = true;
    }

    public void UnPauseHover()
    {
        pauseHover = false;
        startPos = transform.position;
    }
}
