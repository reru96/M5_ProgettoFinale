using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject settingsMenu;

    private void Start()
    {
        settingsMenu.SetActive(false);
    }
    public void StartGame()
    { 
        
        SceneManager.LoadScene(1);
      
    }

    public void ShowOptions()
    {
        settingsMenu.SetActive(true);
    }

    public void HideOptions()
    {
        settingsMenu.SetActive(false);
    }


    public void OnExitGame()
    {
        Application.Quit();
    }
}
