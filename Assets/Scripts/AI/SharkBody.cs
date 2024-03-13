using UnityEngine;

public class SharkBody : MonoBehaviour
{
    SharkBoss boss;

    private void Awake()
    {
        boss = GetComponentInParent<SharkBoss>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            boss.OnTouchedPlayer();
        }
        else
        {
            boss.OnHitSomething(collision.gameObject);
        }
    }
}
