using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(NavMeshAgent))]
public class AgentPathVisual : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private NavMeshAgent agent;
    private NavMeshPath path;

    public Color colorComplete = Color.green;
    public Color colorPartial = Color.yellow;
    public Color colorInvalid = Color.red;

    private Vector3 pendingDestination;
    private bool hasPendingDestination = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
      
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.CalculatePath(hit.point, path);
                DrawPath(path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        pendingDestination = hit.point;
                        hasPendingDestination = true;
                    }
                    else
                    {
                        agent.SetDestination(hit.point);
                        hasPendingDestination = false;
                    }
                }
            }
        }

   
        if (hasPendingDestination && Input.GetKeyUp(KeyCode.LeftShift))
        {
            agent.SetDestination(pendingDestination);
            hasPendingDestination = false;
        }
    }

    void DrawPath(NavMeshPath navPath)
    {
        if (navPath.status == NavMeshPathStatus.PathInvalid || navPath.corners.Length < 2)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = navPath.corners.Length;
        lineRenderer.SetPositions(navPath.corners);

        Color pathColor = colorInvalid;
        switch (navPath.status)
        {
            case NavMeshPathStatus.PathComplete:
                pathColor = colorComplete;
                break;
            case NavMeshPathStatus.PathPartial:
                pathColor = colorPartial;
                break;
            case NavMeshPathStatus.PathInvalid:
                pathColor = colorInvalid;
                break;
        }

        lineRenderer.startColor = pathColor;
        lineRenderer.endColor = pathColor;
    }
}
