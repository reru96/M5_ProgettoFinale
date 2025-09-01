using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private float maxDistance = 100;
    [SerializeField] private bool isWASD = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            isWASD = true;
        }
        else
        {
            isWASD = false;
        }

        if (isWASD)
        {
            Vector3 dir = new Vector3(h, 0, v);
            agent.velocity = dir * agent.speed;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, maxDistance))
                {
                    agent.SetDestination(hit.point);
                    
                }

            }
        }
    }
}
