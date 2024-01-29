using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    Transform respawnPointOverride;
    BoxCollider bc;

    public bool SetOnTriggerEnter { get; set; } = true;
    public int Id { get; set; }

    private void Awake()
    {
        bc = GetComponent<BoxCollider>();

        // automagically find the respawn override
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag != "Respawn") continue;
            respawnPointOverride = transform.GetChild(i);
            break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!SetOnTriggerEnter) return;

        if (other.gameObject.tag == "Player")
        {
            LevelManager.Instance.SetCheckpoint(Id);
            Disable();
        }
    }

    public void Teleport(Transform other)
    {
        if (respawnPointOverride)
        {
            other.position = respawnPointOverride.position;
            other.rotation = respawnPointOverride.rotation;
        }
        else
        {
            other.position = transform.position;
            other.rotation = transform.rotation;
        }
    }

    public void Disable()
    {
        if (bc) bc.enabled = false;
    }
}
