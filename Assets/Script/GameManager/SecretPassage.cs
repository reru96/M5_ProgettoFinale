using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.AI.Navigation;

public class SecretPassage : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] float radius = 1.2f;
    [SerializeField] KeyCode key = KeyCode.E;
    [SerializeField] TextMeshProUGUI promptText; 
    [SerializeField] string promptMessage = "Press E";

    [Header("What to toggle")]
    [SerializeField] GameObject[] enableOnPress;
    [SerializeField] GameObject[] disableOnPress; 

    [Header("NavMesh runtime")]
    [SerializeField] NavMeshSurface[] surfacesToRebake; 

    Transform player;
    bool inRange;

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        player = p ? p.transform : null;
        if (promptText) promptText.enabled = false;
    }

    void Update()
    {
        if (!player) return;

        inRange = Vector3.SqrMagnitude(player.position - transform.position) <= radius * radius;
        if (promptText)
        {
            promptText.text = promptMessage;
            promptText.enabled = inRange;
        }

        if (inRange && Input.GetKeyDown(key))
            Activate();
    }

    void Activate()
    {
        
        foreach (var go in enableOnPress)
        {
            if (!go) continue;
            var obst = go.GetComponent<NavMeshObstacle>();
            if (obst) obst.enabled = false;
            go.SetActive(true);
        }

   
        foreach (var go in disableOnPress)
        {
            if (!go) continue;
            var obst = go.GetComponent<NavMeshObstacle>();
            if (obst) obst.enabled = false;
            go.SetActive(false);
        }

       
        foreach (var s in surfacesToRebake)
            if (s) s.BuildNavMesh();

        if (promptText) promptText.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
    

