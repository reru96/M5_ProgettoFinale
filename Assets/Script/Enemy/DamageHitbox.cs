using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1; 

   
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            
            var life = other.GetComponent<LifeController>();
            if (life != null)
            {
                life.AddHp(-damage);
            }
        }
    }
}
