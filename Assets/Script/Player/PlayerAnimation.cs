using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimation : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
     agent = GetComponent<NavMeshAgent>();  
     anim = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        float speed = agent.velocity.magnitude;
        anim.SetFloat("Speed", speed);
    }
}
