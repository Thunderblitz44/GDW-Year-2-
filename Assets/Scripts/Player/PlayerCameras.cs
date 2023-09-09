using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCameras : MonoBehaviour, IInputExpander
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform freeLookCamera;
    [SerializeField] Transform combatCamera;
    [SerializeField] Transform lockOnCamera;
    [SerializeField] LayerMask lockOnLayers;

    Player playerScript;
    ActionMap actions;

    bool isLockedOn = false;
    GameObject lastUsedCamera;
    RaycastHit[] targets;
    Transform currentTarget;
    List<int> ints = new();
    int i;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LockOnFunction()
    {
        if (isLockedOn)
        {
            isLockedOn = false;
            lockOnCamera.gameObject.SetActive(false);
            lastUsedCamera.gameObject.SetActive(true);
            return;
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hitInfo, 100f, lockOnLayers))
        {
            if (!isLockedOn)
            {
                isLockedOn = true;
                lastUsedCamera = GetCameraTransform().gameObject;
                lastUsedCamera.gameObject.SetActive(false);
                lockOnCamera.gameObject.SetActive(true);
                currentTarget = hitInfo.collider.transform;



                lockOnCamera.GetComponent<CinemachineFreeLook>().LookAt = currentTarget;
            }
        }

        if (isLockedOn)
        {
            // get other targets
            targets = Physics.SphereCastAll(new Ray(transform.position, Vector3.forward), 100f, 100f, lockOnLayers);
        }
    }

    public Transform GetCameraTransform()
    {
        if (freeLookCamera.gameObject.activeSelf)
        {
            return freeLookCamera;
        }
        else if (combatCamera.gameObject.activeSelf)
        {
            return combatCamera;
        }   
        else
        {
            return lockOnCamera;
        }
    }


    #region IInputExpander

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        // Lock on to target
        actions.CameraControl.LockOnToTarget.performed += ctx =>
        {
            LockOnFunction();
        };

        // Cycle Targets
        actions.CameraControl.CycleTargets.performed += ctx =>
        {
            if (!isLockedOn) return;

            if (targets.Length > 0)
            {
                targets = targets.OrderBy((d) => (d.transform.position - transform.position).sqrMagnitude).ToArray();
            }

            for (; i < targets.Length; i++)
            {
                if (targets[i].collider.transform != currentTarget)
                {
                    currentTarget = targets[i].collider.transform;
                    lockOnCamera.GetComponent<CinemachineFreeLook>().LookAt = currentTarget;
                    ints.Add(i);
                    break;
                }
            }
            if (i >= targets.Length) i = 0;
        };

        // toggle cameras
        actions.CameraControl.Aim.started += ctx =>
        {
            if (isLockedOn) LockOnFunction();
            combatCamera.gameObject.SetActive(true);
            freeLookCamera.gameObject.SetActive(false);
        };
        actions.CameraControl.Aim.canceled += ctx =>
        {
            freeLookCamera.gameObject.SetActive(true);
            combatCamera.gameObject.SetActive(false);
        };


        EnableCameraControl();
    }

    #endregion


    public void EnableCameraControl() => actions.CameraControl.Enable();
    public void DisableCameraControl() => actions.CameraControl.Disable();
    public void EnableCameraLockOn() => actions.CameraControl.LockOnToTarget.Enable();
    public void DisableCameraLockOn() => actions.CameraControl.LockOnToTarget.Disable();
    public bool IsLockedOnToATarget() => isLockedOn;
}