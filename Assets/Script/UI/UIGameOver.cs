using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOver : MonoBehaviour
{
   
    public GameObject winMenu;

    private void Start()
    {
       
        winMenu.SetActive(false);
    }

    public void ShowVictory()
    {
        
        Time.timeScale = 0.5f;
        StartCoroutine(ShowWinMenuDelayed());
    }

    private IEnumerator ShowWinMenuDelayed()
    {
        yield return new WaitForSeconds(1f);
        ShowWinMenu();
    }

    private void ShowWinMenu()
    {
        Time.timeScale = 0f;
        winMenu.SetActive(true);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    public void NextScene()
    {
        Time.timeScale = 1f;
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
