using Cinemachine;
using System.Collections;
using UnityEngine;

public class DashAbility : Ability
{
    [SerializeField] float dashDistance = 20f;
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] int maxDashes = 3;
    [SerializeField] float timeLimitBetweenDashes = 1f;
    [SerializeField] float dashCooldown = 5f;
    [SerializeField] AnimationCurve dashCurve;
    [SerializeField] AnimationCurve dashFovIntensityCurve;
    [SerializeField] float dashFovIncrease = 10;
    [SerializeField] AnimationCurve fovRestoreCurve;
    [SerializeField] float fovRestoreSpeed = 5;
    [SerializeField] LayerMask whatIsDashObstacle;
    [SerializeField] GameObject dashMeterPrefab;
    Coroutine currentDashRoutine;
    DashUI dashUI;
    bool isDashing = false;
    int dashes = 0;

    Player playerScript;
    Rigidbody rb;
    Transform body;

    void Start()
    {
        playerScript = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        body = playerScript.movementScript.GetBody();

        dashUI = Instantiate(dashMeterPrefab, GameManager.Instance.canvas).GetComponent<DashUI>();
        dashUI.SetDashVisual(maxDashes);
        dashUI.onDashesRecharged += OnDashRecharged;
    }

    public override void Part1()
    {
        if (isDashing || dashes >= maxDashes) return;
        isDashing = true;
        dashes++;
        dashUI.SpendDash();
        CancelInvoke(nameof(DashTimeout));

        // raycast - make sure there are no obstacles in the way
        float newDist = dashDistance;

        Transform body = playerScript.movementScript.GetBody();
        Transform cam = Camera.main.transform;
        Vector3 end;

        // calculate end for the raycast
        if (playerScript.movementScript.IsMoving()) end = body.position + playerScript.movementScript.GetMoveDirection() * newDist;
        else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

        RaycastHit hit;
        if (Physics.Raycast(body.position, (end - body.position).normalized,
            out hit, dashDistance, whatIsDashObstacle, QueryTriggerInteraction.Ignore))
        {
            newDist = Vector3.Distance(hit.point, body.position) - 0.5f;
        }

        // re-calculate end in case newDist changed
        if (playerScript.movementScript.IsMoving()) end = body.position + playerScript.movementScript.GetMoveDirection() * newDist;
        else end = body.position + new Vector3(cam.forward.x, 0, cam.forward.z) * newDist;

        // lerp it
        dashCurve.ClearKeys();
        dashCurve.AddKey(0, 0);
        dashCurve.AddKey(newDist / dashSpeed, 1);

        if (currentDashRoutine != null) StopCoroutine(currentDashRoutine);
        currentDashRoutine = StartCoroutine(DashRoutine(end));
    }

    IEnumerator DashRoutine(Vector3 endPos)
    {
        playerScript.movementScript.Disable();
        Vector3 startPos = transform.position;
        float time = 0;
        float dashTime = dashCurve.keys[1].time;
        CinemachineFreeLook camera = playerScript.cameraScript.GetFreeLookCamera();
        float startFOV = camera.m_Lens.FieldOfView;
        float targetFOV = StaticUtilities.defaultFOV + dashFovIncrease;
        while (time < dashTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, dashCurve.Evaluate(time += Time.deltaTime));

            // trying to "lerp" fov from whatever it was to the max
            if (startFOV < targetFOV)
                camera.m_Lens.FieldOfView = Mathf.Clamp(startFOV + dashFovIntensityCurve.Evaluate(time / dashTime) * dashFovIncrease, startFOV, targetFOV);
            else
                camera.m_Lens.FieldOfView = Mathf.Clamp(startFOV + dashFovIntensityCurve.Evaluate(time / dashTime) * dashFovIncrease, targetFOV, startFOV);

            yield return null;
        }

        // dash end
        playerScript.movementScript.Enable();
        isDashing = false;

        // cooldown
        if (dashes >= maxDashes)
        {
            dashUI.RechargeDashes(dashCooldown);
        }
        else
        {
            Invoke(nameof(DashTimeout), timeLimitBetweenDashes);
        }

        // restore fov
        time = 0f;
        startFOV = camera.m_Lens.FieldOfView;
        targetFOV = StaticUtilities.defaultFOV;
        while (time < 1)
        {
            // "lerp" back to default fov
            if (targetFOV < startFOV)
                camera.m_Lens.FieldOfView = Mathf.Clamp(startFOV - fovRestoreCurve.Evaluate(time) * dashFovIncrease, targetFOV, startFOV);
            else
                camera.m_Lens.FieldOfView = Mathf.Clamp(startFOV - fovRestoreCurve.Evaluate(time) * dashFovIncrease, startFOV, targetFOV);

            time += Time.deltaTime * fovRestoreSpeed;
            yield return null;
        }
        camera.m_Lens.FieldOfView = StaticUtilities.defaultFOV;
    }

    void OnDashRecharged()
    {
        dashes = 0;
    }

    void DashTimeout()
    {
        dashes = maxDashes;
        dashUI.RechargeDashes(dashCooldown);
    }

    public override void Part2()
    {
    }
}
