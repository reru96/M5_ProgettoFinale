using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerFootSteps : MonoBehaviour
{
    [Header("Footstep Settings")]
    [SerializeField] private string[] footstepKeys;
    [SerializeField] private float stepInterval = 0.5f; 
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private NavMeshAgent agent;
    private float stepTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (agent == null) return;

     
        if (agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f; 
        }
    }

    private void PlayFootstep()
    {
        if (footstepKeys.Length == 0) return;

        string key = footstepKeys[Random.Range(0, footstepKeys.Length)];
        AudioManager.Instance.PlaySfx(key, volume);
    }
}
