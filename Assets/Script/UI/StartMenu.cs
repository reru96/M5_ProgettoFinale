using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        RespawnManager.Instance.ResetTries();
        SceneManager.LoadScene(1);

    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}
