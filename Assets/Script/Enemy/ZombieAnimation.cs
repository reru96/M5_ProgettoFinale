using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAnimation : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private bool isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponentInParent<NavMeshAgent>();
    }

    private void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }

    public void StartAttack(Collider player)
    {
        if (isAttacking) return;

        isAttacking = true;
        animator.SetBool("isAttacking", true);

        StartCoroutine(AttackCoroutine(player));
    }

    public void StopAttack(Collider player)
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    private IEnumerator AttackCoroutine(Collider player)
    {
        
        yield return new WaitForSeconds(2f);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }
}