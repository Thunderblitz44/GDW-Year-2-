using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance;

    Transform _canvas;
    Transform _worldCanvas;
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
            Debug.LogWarning("There were 2 GameManager scripts!");
            Destroy(this);
            return;
        }
        Instance = this;
        _canvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
        _worldCanvas = GameObject.FindGameObjectWithTag("WorldCanvas").transform;
    }
}
