using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] Transform canvas;
    [SerializeField] Transform worldCanvas;
    [SerializeField] Camera lobbyCamera;
    public List<Transform> renderedGrappleTargets = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void DisableLobbyCamera()
    {
        lobbyCamera.gameObject.SetActive(false);
    }

    public Transform GetCanvas() => canvas;
    public Transform GetWorldCanvas() => worldCanvas;
    public Camera GetLobbyCamera() => lobbyCamera;
}
