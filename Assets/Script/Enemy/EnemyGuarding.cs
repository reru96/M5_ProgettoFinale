using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGuarding : EnemyPathController
{

    [Header("Guard Settings")]
    [SerializeField] private float rotationAngle = 45f;   
    [SerializeField] private float rotationSpeed = 60f;   
    [SerializeField] private float pauseTime = 1f;        

    private Quaternion leftRotation;
    private Quaternion rightRotation;
    private Quaternion targetRotation;
    private bool lookingRight = true;
    private float pauseTimer = 0f;

    protected override void Start()
    {
        base.Start();

        
        leftRotation = Quaternion.Euler(0, transform.eulerAngles.y - rotationAngle, 0);
        rightRotation = Quaternion.Euler(0, transform.eulerAngles.y + rotationAngle, 0);
        targetRotation = rightRotation;
    }

    protected override void HandlePatrolling()
    {
      
        GuardLookAround();

        if (canSeePlayerNow)
        {
            ChangeState(State.Chasing);
        }
    }

    private void GuardLookAround()
    {
        if (pauseTimer > 0)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

       
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

       
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            lookingRight = !lookingRight;
            targetRotation = lookingRight ? rightRotation : leftRotation;
            pauseTimer = pauseTime;
        }
    }
}