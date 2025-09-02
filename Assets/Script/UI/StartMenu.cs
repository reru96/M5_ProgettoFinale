using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
        RespawnManager.Instance.ResetTries();

    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}
