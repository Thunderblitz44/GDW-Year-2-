using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum DamageTypes
{
    physical,
    magic
}

public class GameSettings : NetworkBehaviour
{
    public static GameSettings instance;

    public float damageOverTimeInterval = 0.25f;
    public float defaultFOV = 50f;
    [SerializeField] Transform canvas;
    [SerializeField] Transform worldCanvas;
    [SerializeField] Camera lobbyCamera;
    [SerializeField] Image grappleLockonIcon;
    public List<Transform> renderedGrappleTargets = new();

    public Color physicalDamageColor = Color.white;
    public Color magicDamageColor = Color.magenta;

    public static Vector2 centerOfScreen = new Vector2(Screen.width/2, Screen.height/2);


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

    public void SortGrappleTargetsByDistance()
    {
        if (!Camera.main) return;
        renderedGrappleTargets = renderedGrappleTargets.OrderBy(x => Vector2.Distance(centerOfScreen, (Vector2)Camera.main.WorldToScreenPoint(x.transform.position))).ToList();
    }


    public Transform GetCanvas() => canvas;
    public Transform GetWorldCanvas() => worldCanvas;
    public Camera GetLobbyCamera() => lobbyCamera;
    public Image GetGrappleLockonIcon() => grappleLockonIcon;
}
