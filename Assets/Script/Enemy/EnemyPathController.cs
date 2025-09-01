using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathController : MonoBehaviour
{

    [Header("Patrol Settings")]
    [SerializeField]protected Transform[] waypoints;
    [SerializeField]protected float waitTimeAtWaypoint = 1f;
    [SerializeField]protected float alertDuration = 5f;
    [SerializeField]protected float alertTimer;
    [SerializeField]protected float patrollingSpeed = 1.5f;
    [SerializeField]protected float chasingSpeed = 3.5f;
    [SerializeField]protected float alertSpeed = 1.5f;

    [Header("Detection Settings")]
    [SerializeField]protected float detectionRadius = 5f;
    [SerializeField]protected float viewAngle = 90f;
    [SerializeField]protected int rayCount = 50;
    [SerializeField]protected LayerMask playerLayer;
    [SerializeField]protected float nearbyEnemyRadius = 10f;

    [SerializeField] protected LineRenderer visionConeRenderer;
    protected NavMeshAgent agent;
    protected int currentWaypoint = 0;
    [SerializeField] protected Transform targetPlayer;
    protected Coroutine waitCoroutine;

    protected State currentState = State.Patrolling;
    public State CurrentState => currentState;
    public event Action<State> OnStateChanged;

    protected virtual void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        InitializeVisionCone();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (targetPlayer == null && playerObj != null)
            targetPlayer = playerObj.transform;

        GoToNextWaypoint();
    }

    protected virtual void Update()
    {
        UpdateVisionCone();
        CheckPlayerDetection();
        HandleStates();
    }

    protected virtual void InitializeVisionCone()
    {
        if (visionConeRenderer == null) visionConeRenderer = gameObject.AddComponent<LineRenderer>();
        visionConeRenderer.useWorldSpace = false;   
        visionConeRenderer.startWidth = 0.1f;
        visionConeRenderer.endWidth = 0.1f;
        visionConeRenderer.loop = true;            
        visionConeRenderer.positionCount = rayCount + 2;
    }

    protected virtual void UpdateVisionCone()
    {
        visionConeRenderer.positionCount = rayCount + 2;
        Vector3[] points = new Vector3[visionConeRenderer.positionCount];
        points[0] = Vector3.zero; 

        float currentAngle = -viewAngle / 2f;
        float angleIncrement = viewAngle / (rayCount - 1);

        for (int i = 1; i <= rayCount; i++)
        {
            Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
            points[i] = dir * detectionRadius;
            currentAngle += angleIncrement;
        }

        points[rayCount + 1] = Vector3.zero; 
        visionConeRenderer.SetPositions(points);
    }

    protected virtual void CheckPlayerDetection()
    {
        if (targetPlayer == null) return;

        if (IsTargetInVisionCone(targetPlayer))
        {
            if (currentState == State.Patrolling && waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
                waitCoroutine = null;
            }
            ChangeState(State.Chasing);
        }
        else
        {
           
            if (currentState == State.Chasing)
            {
                ChangeState(State.Alert);
                alertTimer = alertDuration;  
            }
        }
    }

    protected virtual bool IsTargetInVisionCone(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToTarget);

        if (angle > viewAngle / 2f) return false;
        if (Vector3.Distance(transform.position, target.position) > detectionRadius) return false;

        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, dirToTarget);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionRadius))
        {
            if (hit.transform != target) return false;
        }

        return true;
    }

    protected virtual void HandleStates()
    {
        switch (currentState)
        {
            case State.Patrolling:
                HandlePatrolling();
                break;

            case State.Chasing:
                HandleChasing();
                break;

            case State.Alert:
                HandleAlert();
                break;
        }
    }

    protected virtual void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        OnStateChanged?.Invoke(currentState); 
    }


    protected virtual void HandlePatrolling()
    {
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            waitCoroutine == null)
        {
            waitCoroutine = StartCoroutine(WaitAndGoToNext());
        }
    }

    protected virtual void HandleChasing()
    {
        if (targetPlayer != null)
        { 
            agent.speed = chasingSpeed;
            agent.SetDestination(targetPlayer.position);
           
            if (!IsTargetInVisionCone(targetPlayer))
            {
                ChangeState(State.Patrolling);
                GoToNextWaypoint();
            }
        }
    }

    protected virtual void HandleAlert()
    {
        agent.ResetPath();
        agent.speed = alertSpeed;
        agent.SetDestination(targetPlayer.position);

        if (targetPlayer != null && IsTargetInVisionCone(targetPlayer))
        {
            ChangeState(State.Chasing)  ;
            return;
        }

        AlertNearbyEnemies();

        if (alertTimer <= 0f)
        {
            ChangeState(State.Patrolling);
            GoToNextWaypoint();
        }
        else
        {
            alertTimer -= Time.deltaTime;
        }
    }

    protected virtual void AlertNearbyEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, nearbyEnemyRadius);
        foreach (Collider col in colliders)
        {
            EnemyPathController otherEnemy = col.GetComponent<EnemyPathController>();
            if (otherEnemy != null && otherEnemy != this)
            {
                otherEnemy.ReceiveAlert(targetPlayer);
            }
        }
    }

    public virtual void ReceiveAlert(Transform player)
    {
        targetPlayer = player;
        currentState = State.Alert;
        alertTimer = alertDuration;
    }

    protected IEnumerator WaitAndGoToNext()
    {
        yield return new WaitForSeconds(waitTimeAtWaypoint);
        GoToNextWaypoint();
        waitCoroutine = null;
    }

    protected void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;  
        agent.speed = patrollingSpeed;
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (!Application.isPlaying || visionConeRenderer == null) return;

        Gizmos.color = Color.yellow;
        for (int i = 1; i < visionConeRenderer.positionCount - 1; i++)
        {
            Vector3 worldPos = transform.TransformPoint(visionConeRenderer.GetPosition(i));
            Gizmos.DrawLine(transform.position, worldPos);
        }
    }
}