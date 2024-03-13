using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class LightningBall : MonoBehaviour
{
     private Transform playerTransform;
    private bool isDisabled = false;
    private bool isScaling = false;
    private bool isMoving = false;
    private float moveStartTime;
    private Vector3 initialPosition;
    MeleeHitBox[] RangedAttack;
  
    public int RangedAttackDamage;
    public Vector2 RangedKnockback;

    private void Start()
    {
        playerTransform = LevelManager.PlayerTransform;
        RangedAttack = GetComponents<MeleeHitBox>();
        foreach (var trigger in RangedAttack)
        {
            trigger.damage = RangedAttackDamage;
            trigger.knockback = RangedKnockback;
        }
        
        // Generate a random delay before starting movement
        GenerateRandomDelay();
    }

    private void Update()
    {
        if (isDisabled || !isMoving)
            return;

        // Move towards player
        float movementSpeed = 4f;
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, movementSpeed * Time.deltaTime);

        // Check if close to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (!isScaling && distanceToPlayer <= 0.5f)
        {
            // Start scaling and disable coroutine
            StartCoroutine(ScaleAndDisable());
        }
    }

    private void GenerateRandomDelay()
    {
        float randomDelay = Random.Range(0f, 5f);
        Invoke("StartMovingTowardsPlayer", randomDelay);
    }

    private void StartMovingTowardsPlayer()
    {
        isMoving = true;
        moveStartTime = Time.time;
    }

    private IEnumerator ScaleAndDisable()
    {
        isScaling = true;

        // Scale up quickly to 2
        float scaleSpeed = 0.5f;
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one * 0f;

        while (elapsedTime < 0.2f)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / 0.2f);
            elapsedTime += Time.deltaTime * scaleSpeed;
            yield return null;
        }

        // Delay for 1 second before disabling the object
        yield return new WaitForSeconds(1f);

        // Disable the object
        gameObject.SetActive(false);
    }

    private void OnParticleCollision(GameObject other)
    {
        StaticUtilities.TryToDamage(other, RangedAttackDamage);
    }
}
