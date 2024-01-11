using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class GolemBossScript : MonoBehaviour
{

    private NavMeshAgent GolemBossAgent;

    private Animator GolemBossAnimator;
    // Start is called before the first frame update
    void Start()
    {
        GolemBossAnimator = GetComponent<Animator>();
        GolemBossAgent = GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
