using System;
using System.Collections;
using UnityEngine;

public class PlayerCameras : MonoBehaviour, IPlayerStateListener, IInputExpander
{
    [SerializeField] Transform freeLookCamera;
    [SerializeField] Transform combatCamera;
    [SerializeField] LayerMask lockOnLayers;

    Player playerScript;
    ActionMap actions;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            Vector3 dir = Camera.main.transform.forward;
            Vector3 p1 = Camera.main.transform.position;
            Vector3 p2 = p1 + dir * 20f;
            Physics.CapsuleCast(p1, p2, 5f, dir, out hitInfo, 100f, lockOnLayers);

            Debug.Log(hitInfo.collider);
        };

        // Look
        actions.CameraControl.Look.performed += ctx =>
        {
            if (Cursor.visible) 
            {
                Debug.Log("cursor visible");
            }

            if ((playerScript.GetMovementScript().IsMoving() && !playerScript.isInCombat) || playerScript.isInCombat)
            {
                // Recalculate body rotation
                playerScript.GetMovementScript().RecalculateBodyRotation();
            }
        };

        //EnableCameraLockOn();
        EnableCameraControl();
        DisableCameraLockOn();
    }

    #endregion

    public void EnableCameraControl() => actions.CameraControl.Enable();
    public void DisableCameraControl() => actions.CameraControl.Disable();
    public void EnableCameraLockOn() => actions.CameraControl.LockOnToTarget.Enable();
    public void DisableCameraLockOn() => actions.CameraControl.LockOnToTarget.Disable();
}