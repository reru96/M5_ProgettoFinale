using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGuarding : EnemyPathController
{
    [Header("Guard Settings")]
    public float rotationSpeed = 45f;
    public float rotationAngle = 45f;
    private float startYaw;
    private bool rotatingRight = true;

    protected override void Start()
    {
        base.Start(); 

        startYaw = transform.eulerAngles.y;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

  
    protected override void HandlePatrolling()
    {
        float targetRotation = rotatingRight ? startYaw + rotationAngle : startYaw - rotationAngle;
        float step = rotationSpeed * Time.deltaTime;
        float newY = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation, step);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, newY, transform.eulerAngles.z);

        if (Mathf.Abs(Mathf.DeltaAngle(newY, targetRotation)) < 0.5f)
            rotatingRight = !rotatingRight;
    }
}