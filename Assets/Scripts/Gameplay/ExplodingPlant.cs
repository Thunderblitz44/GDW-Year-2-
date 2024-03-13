using UnityEngine;

public class ExplodingPlant : DamageableEntity
{
    [SerializeField] int damage = 50;
    MeshRenderer mr;
    GameObject expl;

    protected override void Awake()
    {
        base.Awake();
        expl = transform.GetChild(0).gameObject;
        expl.GetComponent<AttackTrigger>().onTriggerEnter += TriggerEnter;
        expl.SetActive(false);
        mr = transform.GetChild(1).GetComponent<MeshRenderer>();
    }

    protected override void OnHealthZeroed()
    {
        mr.enabled = false;
        expl.SetActive(true);
        Invoke(nameof(Die), 0.38f);
    }

    void TriggerEnter(Collider other)
    {
        StaticUtilities.TryToDamage(other.gameObject, damage);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
