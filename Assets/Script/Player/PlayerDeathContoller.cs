using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathContoller : MonoBehaviour
{
    private LifeController life;

    private void Start()
    {
        life = GetComponent<LifeController>();
    }

    private void Update()
    {
        if (life != null && life.GetHp() <= 0)
        {
            RespawnManager.Instance.PlayerDied();
        }
    }
}
