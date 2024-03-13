using System.Collections.Generic;
using UnityEngine;

public class Level2Puzzle : MonoBehaviour
{
    private Animator Animator;
    [SerializeField] int enemyCount;
    readonly List<GameObject> enemies = new();

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        Bounds bounds = GetComponent<BoxCollider>().bounds;
        for (int i = 0; i < enemyCount; i++)
        {
            int e = Random.Range(0, LevelManager.Instance.LevelEnemyList.Count);
            Vector3 spawnPoint = LevelManager.GetRandomEnemySpawnPoint(bounds);
            enemies.Add(Instantiate(LevelManager.Instance.LevelEnemyList[e], spawnPoint, Quaternion.LookRotation(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)))));
        }

        foreach (var item in enemies)
        {
            item.SetActive(false);
        }
    }

    public void GreenGemActivation()
    {
        //Player Completes Green Gem Encounter
        Animator.SetTrigger("Green Gem");
    }
    
    public void YellowGemActivation()
    {
        //Same thing with other colors
        Animator.SetTrigger("Yellow Gem");
    }
    public void BlueGemActivation()
    {
        Animator.SetTrigger("Blue Gem");
    }

    public void OpenSesame()
    {
        Animator.SetTrigger("Door");
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var item in enemies)
        {
            item.SetActive(true);
        }

        GetComponent<BoxCollider>().enabled = false;
    }

}
