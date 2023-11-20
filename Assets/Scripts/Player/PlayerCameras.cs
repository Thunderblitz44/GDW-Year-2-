using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerCameras : NetworkBehaviour, IInputExpander
{
    [SerializeField] GameObject cameraRig;
    CinemachineFreeLook freeLookCamera;
    [SerializeField] Transform aim;
    [SerializeField] LayerMask lockOnLayers;

    ActionMap actions;

    bool isLockedOn = false;
    GameObject lastUsedCamera;
    RaycastHit[] targets;
    Transform currentTarget;
    List<int> ints = new();
    int i;

    void Start()
    {
        if (!IsOwner) return;
        
        freeLookCamera = Instantiate(cameraRig).transform.GetChild(1).GetComponent<CinemachineFreeLook>();
        freeLookCamera.LookAt = transform;
        freeLookCamera.Follow = transform;
        freeLookCamera.m_Lens.FieldOfView = StaticUtilities.defaultFOV;

        Application.focusChanged += (bool isFocused) => 
        { 
            if (isFocused)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        };
    }

    void LockOnFunction()
    {
        if (isLockedOn)
        {
            isLockedOn = false;
            //lockOnCamera.gameObject.SetActive(false);
            lastUsedCamera.gameObject.SetActive(true);
            return;
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hitInfo, 100f, lockOnLayers))
        {
            if (!isLockedOn)
            {
                isLockedOn = true;
                lastUsedCamera = GetFreeLookCamera().gameObject;
                lastUsedCamera.gameObject.SetActive(false);
                //lockOnCamera.gameObject.SetActive(true);
                currentTarget = hitInfo.collider.transform;



                //lockOnCamera.GetComponent<CinemachineFreeLook>().LookAt = currentTarget;
            }
        }

        if (isLockedOn)
        {
            // get other targets
            targets = Physics.SphereCastAll(new Ray(transform.position, Vector3.forward), 100f, 100f, lockOnLayers);
        }
    }

    public CinemachineFreeLook GetFreeLookCamera()
    {
        return freeLookCamera;
    }


    #region IInputExpander

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        this.actions = actions;

        // Lock on to target
        actions.CameraControl.LockOnToTarget.performed += ctx =>
        {
            //LockOnFunction();
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
                    //lockOnCamera.GetComponent<CinemachineFreeLook>().LookAt = currentTarget;
                    ints.Add(i);
                    break;
                }
            }
            if (i >= targets.Length) i = 0;
        };

        // toggle cameras
        actions.CameraControl.Aim.started += ctx =>
        {
        };
        actions.CameraControl.Aim.canceled += ctx =>
        {
        };

        actions.General.Escape.performed += ctx => 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        };

        actions.General.Escape.Enable();

        EnableCameraControl();
    }

    #endregion


    public void EnableCameraControl() => actions.CameraControl.Enable();
    public void DisableCameraControl() => actions.CameraControl.Disable();
    public void EnableCameraLockOn() => actions.CameraControl.LockOnToTarget.Enable();
    public void DisableCameraLockOn() => actions.CameraControl.LockOnToTarget.Disable();
    public bool IsLockedOnToATarget() => isLockedOn;
}