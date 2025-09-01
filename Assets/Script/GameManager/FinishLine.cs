using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public static event Action OnPlayerFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerFinished?.Invoke();
        }
    }

}
