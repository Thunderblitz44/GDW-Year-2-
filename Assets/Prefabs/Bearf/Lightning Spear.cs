using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSpear : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 directionToPlayer;
    private float speed = 10f;
    private float destroyDelay = 5f;
    private bool isDestroyed = false;

    void Start()
    {
        playerTransform = LevelManager.PlayerTransform;
        directionToPlayer = (playerTransform.position - transform.position).normalized;
        Destroy(gameObject, destroyDelay);
    }

    void Update()
    {
        if (!isDestroyed)
        {
            // Rotate the spear to face the player
            transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

            // Move the spear towards the player's position
            transform.position += directionToPlayer * speed * Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        isDestroyed = true; // Set to true to avoid further updates after destruction
    }
}
