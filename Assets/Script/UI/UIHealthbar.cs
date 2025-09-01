using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthbar : MonoBehaviour
{
    [SerializeField] private LifeController target;   
    [SerializeField] private Image fillImage;         

    private void Update()
    {
        if (target != null && fillImage != null)
        {
            float ratio = (float)target.GetHp() / target.GetMaxHp();
            fillImage.fillAmount = ratio;
        }
    }
}
