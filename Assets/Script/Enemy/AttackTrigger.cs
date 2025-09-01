using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    private ZombieAnimation zombieParent;

    private void Awake()
    {
        zombieParent = GetComponentInParent<ZombieAnimation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zombieParent.StartAttack(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zombieParent.StopAttack();
        }
    }
}
