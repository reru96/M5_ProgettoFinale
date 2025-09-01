using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILives : MonoBehaviour
{
    [SerializeField] private RespawnManager respawnManager;
    [SerializeField] private GameObject lifeIconPrefab;
    private List<GameObject> icons = new List<GameObject>();

    private void Awake()
    {
        respawnManager = FindAnyObjectByType<RespawnManager>();
    }
    void Start()
    {
        InitIcons();
        UpdateLives();
    }

    private void InitIcons()
    {
      
        for (int i = 0; i < respawnManager.MaxTry; i++)
        {
            GameObject newIcon = Instantiate(lifeIconPrefab, transform);
            icons.Add(newIcon);
        }
    }

    public void UpdateLives()
    {
        if (respawnManager == null)
            respawnManager = RespawnManager.Instance;

        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].SetActive(i < respawnManager.LeftTry);
        }
    }
}
