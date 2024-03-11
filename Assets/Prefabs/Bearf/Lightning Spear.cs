using UnityEngine;

public class LightningSpear : MonoBehaviour
{
    [SerializeField] int damage = 20;
    [SerializeField] float speed = 80f;
    [SerializeField] float damageRadius = 3f;
    [SerializeField] float stuckInGroundTime = 10f;
    [SerializeField] LayerMask playerLayer;
    Vector3 directionToPlayer, start, end, currentPos, lastPos;
    RaycastHit hit;
    float dist, t;
    bool deathCycle, hitPlayer;

    void Start()
    {
        directionToPlayer = (LevelManager.PlayerTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
        start = transform.position;
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, 1000f, StaticUtilities.groundLayer, QueryTriggerInteraction.Ignore))
        {
            end = hit.point;
            dist = Vector3.Distance(start, end);
        }
    }

    void Update()
    {
        if (!deathCycle)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(start, end, t / dist);
            
            if (!hitPlayer)
            {
                currentPos = transform.position;
                if (Physics.CapsuleCast(lastPos, currentPos, damageRadius, currentPos - lastPos, out hit, 20f, playerLayer, QueryTriggerInteraction.Ignore))
                {
                    hitPlayer = true;
                    StaticUtilities.TryToDamage(hit.transform.gameObject, damage);
                }
                lastPos = transform.position;
            }
        }

        if (t > dist && !deathCycle)
        {
            deathCycle = true;
            Destroy(gameObject, stuckInGroundTime);
        }
    }
}
