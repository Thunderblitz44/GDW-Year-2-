using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") LevelManager.Instance.Respawn();
        else
        {
            Destroy(other.gameObject);
        }
    }
}
