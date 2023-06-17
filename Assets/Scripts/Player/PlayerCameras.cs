using Cinemachine;
using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerCameras : MonoBehaviour, IPlayerStateListener, IInputExpander
{
    [SerializeField] Transform freeLookCamera;
    [SerializeField] Transform combatCamera;
    [SerializeField] LayerMask lockOnLayers;
    [SerializeField] GameObject sphere;

    Player playerScript;
    ActionMap actions;
    Transform aimSphere;
    bool isLockedOn;

    Transform originalFreeLookLookAt;
    Transform originalCombatLookAt;


    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        aimSphere = Instantiate(sphere).transform;
        originalFreeLookLookAt = freeLookCamera.GetComponent<CinemachineFreeLook>().LookAt;
        originalCombatLookAt = combatCamera.GetComponent<CinemachineFreeLook>().LookAt;
    }

    public Transform GetCameraTransform()
    {
        if (freeLookCamera.gameObject.activeSelf)
        {
            return freeLookCamera;
        }

        return combatCamera;
    }

    #region IPlayerStateListener

    public void SetCombatState()
    {
        combatCamera.gameObject.SetActive(true);
        freeLookCamera.gameObject.SetActive(false);
    }

    public void SetFreeLookState()
    {
        freeLookCamera.gameObject.SetActive(true);
        combatCamera.gameObject.SetActive(false);
    }

    #endregion

    #region IInputExpander

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        actions.CameraControl.LockOnToTarget.performed += ctx =>
        {
            // does not work

            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hitInfo, 100f, lockOnLayers))
            {
                //playerScript.SetFreeLookState();
                if (!isLockedOn)
                {
                    freeLookCamera.GetComponent<CinemachineFreeLook>().LookAt = hitInfo.transform;
                    combatCamera.GetComponent<CinemachineFreeLook>().LookAt = hitInfo.transform;
                }
                else
                {
                    freeLookCamera.GetComponent<CinemachineFreeLook>().LookAt = originalFreeLookLookAt;
                    combatCamera.GetComponent<CinemachineFreeLook>().LookAt = originalCombatLookAt;
                }


                //RaycastHit hit2;
                //if (Physics.Raycast(new Ray(hitInfo.point, hitInfo.transform.position - hitInfo.point), out hit2,3f, lockOnLayers, QueryTriggerInteraction.Ignore))
                //{
                //    //Destroy(Instantiate(sphere, hit2.point, Quaternion.identity), 3.0f);
                //}
            }

        };

        // Look
        actions.CameraControl.Look.performed += ctx =>
        {
            if (Cursor.visible) 
            {
                //Debug.Log("cursor visible");
            }

            if ((playerScript.GetMovementScript().IsMoving() && !playerScript.isInCombat) || playerScript.isInCombat)
            {
                // Recalculate body rotation
                playerScript.GetMovementScript().RecalculateBodyRotation();
            }

            //RaycastHit hitInfo;
            //if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hitInfo, 100f, lockOnLayers))
            //{
            //    RaycastHit hit2;
            //    if (Physics.Raycast(new Ray(hitInfo.point, hitInfo.transform.position - hitInfo.point), out hit2, 3f, lockOnLayers, QueryTriggerInteraction.Ignore))
            //    {
            //        aimSphere.position = hit2.point;
            //        //Destroy(Instantiate(sphere, hit2.point, Quaternion.identity), 3.0f);
            //    }
            //}
        };

        //EnableCameraLockOn();
        EnableCameraControl();
    }

    #endregion




    public void EnableCameraControl() => actions.CameraControl.Enable();
    public void DisableCameraControl() => actions.CameraControl.Disable();
    public void EnableCameraLockOn() => actions.CameraControl.LockOnToTarget.Enable();
    public void DisableCameraLockOn() => actions.CameraControl.LockOnToTarget.Disable();
}