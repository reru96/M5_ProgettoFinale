using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class EnemyPathController : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] protected Transform targetPlayer;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform eyes; 

    [Header("Patrol Settings")]
    [SerializeField] protected Transform[] waypoints;
    [SerializeField] protected float waitTimeAtWaypoint = 1f;
    [SerializeField] protected float patrollingSpeed = 1.5f;
    [SerializeField] protected float chasingSpeed = 3.5f;
    [SerializeField] protected float alertSpeed = 1.5f;

    [Header("Visione")]
    public float viewRadius = 12f;
    [Range(1f, 360f)]
    public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public float checkInterval = 0.1f;

    [Header("Comportamento Alert")]
    [SerializeField] protected float alertDuration = 5f;
    [SerializeField] protected float nearbyEnemyRadius = 10f;

    [SerializeField] protected float stoppingDistance = 0.5f;

    [Header("Debug Visivo")]
    [SerializeField] protected LineRenderer visionConeRenderer;
    [SerializeField] protected int rayCount = 50;

    protected int currentWaypoint = 0;
    protected Coroutine waitCoroutine;
    protected State currentState = State.Patrolling;
    public State CurrentState => currentState;
    public event Action<State> OnStateChanged;

    public bool canSeePlayerNow;
    public float lastSeenTime = -999f;
    Vector3 lastSeenPosition;
    public float nextCheckTime;
    public float alertTimer;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (targetPlayer == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) targetPlayer = playerObj.transform;
        }
        InitializeVisionCone();
        GoToNextWaypoint();
    }

    protected virtual void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            canSeePlayerNow = CanSeePlayer();
            if (canSeePlayerNow && targetPlayer != null)
            {
                lastSeenTime = Time.time;
                lastSeenPosition = targetPlayer.position;
            }
        }

        UpdateVisionCone();
        HandleStates();
    }

    protected virtual void InitializeVisionCone()
    {
        if (visionConeRenderer == null)
            visionConeRenderer = gameObject.AddComponent<LineRenderer>();

        visionConeRenderer.useWorldSpace = false;
        visionConeRenderer.startWidth = 0.05f;
        visionConeRenderer.endWidth = 0.05f;
        visionConeRenderer.loop = true;
        visionConeRenderer.positionCount = rayCount + 2;
        visionConeRenderer.material = new Material(Shader.Find("Sprites/Default"));
        visionConeRenderer.startColor = new Color(1f, 1f, 0f, 0.25f);
        visionConeRenderer.endColor = new Color(1f, 1f, 0f, 0.25f);
    }

    protected virtual void UpdateVisionCone()
    {
        if (visionConeRenderer == null) return;

        Vector3[] points = new Vector3[rayCount + 2];
        points[0] = Vector3.zero;

        float currentAngle = -viewAngle / 2f;
        float angleIncrement = viewAngle / (rayCount - 1);

        for (int i = 1; i <= rayCount; i++)
        {
            Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
            Vector3 localPoint = dir * viewRadius;
            points[i] = localPoint;
            currentAngle += angleIncrement;
        }

        points[rayCount + 1] = Vector3.zero;
        visionConeRenderer.positionCount = points.Length;
        visionConeRenderer.SetPositions(points);
    }



    protected virtual bool CanSeePlayer()
    {
        if (targetPlayer == null) return false;

        Vector3 eyePos = eyes ? eyes.position : transform.position + Vector3.up * 1.6f;
        Vector3 toTarget = targetPlayer.position - eyePos;
        float distToTarget = toTarget.magnitude;


        if (distToTarget > viewRadius) return false;

      
        Vector3 dirToTarget = toTarget.normalized;
        float angleToTarget = Vector3.Angle(GetForwardOnPlane(), Vector3.ProjectOnPlane(dirToTarget, Vector3.up));
        if (angleToTarget > viewAngle * 0.5f) return false;


        if (Physics.Raycast(eyePos, dirToTarget, out RaycastHit hit, distToTarget, obstructionMask))
            return false;

       
        if (Physics.Raycast(eyePos, dirToTarget, out RaycastHit hitAny, distToTarget, ~0, QueryTriggerInteraction.Ignore))
        {
            bool isTargetLayer = (targetMask.value & (1 << hitAny.collider.gameObject.layer)) != 0;
            if (!isTargetLayer) return false;
        }

        return true;
    }

    Vector3 GetForwardOnPlane()
    {
        Vector3 fwd = transform.forward;
        fwd.y = 0f;
        return fwd.sqrMagnitude > 0.0001f ? fwd.normalized : Vector3.forward;
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
        if (canSeePlayerNow)
        {
            ChangeState(State.Chasing);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && waitCoroutine == null)
            waitCoroutine = StartCoroutine(WaitAndGoToNext());
    }

    protected virtual void HandleChasing()
    {
        agent.speed = chasingSpeed;
        agent.stoppingDistance = stoppingDistance;

        if (canSeePlayerNow)
        {
            lastSeenPosition = targetPlayer.position;
            agent.SetDestination(lastSeenPosition);
        }
        else
        {
            ChangeState(State.Alert);
            alertTimer = alertDuration;
        }
    }

    protected virtual void HandleAlert()
    {
        agent.speed = alertSpeed;
        agent.stoppingDistance = 0f;

        
        if (canSeePlayerNow)
        {
            lastSeenPosition = targetPlayer.position;
            ChangeState(State.Chasing);
            return;
        }

     
        if (!agent.hasPath)
        {
            agent.SetDestination(lastSeenPosition);
        }

       
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
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
    }

    protected virtual void AlertNearbyEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, nearbyEnemyRadius);
        foreach (Collider col in colliders)
        {
            EnemyPathController otherEnemy = col.GetComponent<EnemyPathController>();
            if (otherEnemy != null && otherEnemy != this)
                otherEnemy.ReceiveAlert(targetPlayer);
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
        agent.stoppingDistance = 0f;
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Vector3 origin = eyes ? eyes.position : transform.position + Vector3.up * 1.6f;

        Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
        DrawWireDisc(origin, Vector3.up, viewRadius);

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2f);
        Vector3 rightBoundary = DirFromAngle(+viewAngle / 2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + leftBoundary * viewRadius);
        Gizmos.DrawLine(origin, origin + rightBoundary * viewRadius);

        if (targetPlayer)
        {
            Gizmos.color = canSeePlayerNow ? Color.green : Color.red;
            Gizmos.DrawLine(origin, targetPlayer.position);
        }
    }

    Vector3 DirFromAngle(float angleDegrees)
    {
        float rad = (transform.eulerAngles.y + angleDegrees) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }

    void DrawWireDisc(Vector3 center, Vector3 up, float radius, int segments = 32)
    {
        Vector3 normal = up.normalized;
        Vector3 forward = Vector3.ProjectOnPlane(GetForwardOnPlane(), normal).normalized;
        if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;

        Vector3 right = Vector3.Cross(normal, forward);
        Vector3 prev = center + (forward * radius);
        for (int i = 1; i <= segments; i++)
        {
            float t = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 p = center + (forward * Mathf.Cos(t) + right * Mathf.Sin(t)) * radius;
            Gizmos.DrawLine(prev, p);
            prev = p;
        }
    }

}


