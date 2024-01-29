using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class GolemBossScript : MonoBehaviour
{
    NavMeshAgent GolemBossAgent;
    Animator GolemBossAnimator;



    void Awake()
    {
        GolemBossAnimator = GetComponent<Animator>();
        GolemBossAgent = GetComponentInParent<NavMeshAgent>();
    }

}
