using System;
using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal otherPortal;
    SpriteRenderer sr;
    [SerializeField] float dissTime = 0.5f;
    [SerializeField] float dissDelay = 0.5f;
    Action onPortalContact;

  

    void Awake()   
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        onPortalContact?.Invoke();
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
