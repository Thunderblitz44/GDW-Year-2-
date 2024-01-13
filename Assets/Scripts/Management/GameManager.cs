using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour 
{
    public static GameManager Instance;

    [SerializeField] Transform _canvas;
    [SerializeField] Transform _worldCanvas;
    public List<LevelManager> levels = new();

    public List<Transform> renderedGrappleTargets = new();

    public List<GameObject> playerPrefabs;


    public Transform worldCanvas { get { return _worldCanvas; } } 
    public Transform canvas { get { return _canvas; } }
    public bool isGamePaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

}
