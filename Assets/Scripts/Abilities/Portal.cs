using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Portal : NetworkBehaviour
{
    int maxUses;
    int uses;
    Portal otherPortal;
    SpriteRenderer sr;
    public float dissTime = 0.5f;
    public float dissDelay = 0.5f;
    public float portalHeight = 1;
    float time;

    

    public override void OnNetworkSpawn()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (uses != 0)
        {
            time += Time.deltaTime;
            float f = 0.1f * Mathf.Sin(uses * 5f * time) + 0.9f;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, f);
        }
    }

    public void Init(int maxUses, Portal otherPortal)
    {
        this.otherPortal = otherPortal;
        this.maxUses = maxUses;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.position = otherPortal.transform.position - otherPortal.transform.forward;

        otherPortal.uses++;
        if (++uses == maxUses) 
        {
            Die();
            otherPortal.Die();
        }
    }

    public void Die()
    {
        Destroy(GetComponent<BoxCollider>());
        StartCoroutine(Dissipate());
    }

    IEnumerator Dissipate()
    {
        yield return new WaitForSeconds(dissDelay);

        for (float i = 0; i/dissTime < 1; i+=Time.deltaTime)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1 - (i / dissTime));
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
