using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheonix : MonoBehaviour
{
    private Animator PheonixAnimator;
    // Start is called before the first frame update
    void Start()
    {
        PheonixAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CastAttack()
    {
        PheonixAnimator.SetBool("IsCasting", true);
        PheonixAnimator.SetTrigger("TriggerCast");
    }

    public void EndAttack()
    {
        PheonixAnimator.SetBool("IsCasting", false);
    }
}
