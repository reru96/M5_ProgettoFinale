using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIVictoryMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "StartMenu";

    private void OnEnable()
    {
      
        FinishLine.OnPlayerFinished += ShowVictoryMenu;
    }

    private void OnDisable()
    {
       
        FinishLine.OnPlayerFinished -= ShowVictoryMenu;
    }

    private void Start()
    {
        if (victoryCanvas != null)
            victoryCanvas.SetActive(false);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(LoadNextLevel);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    public void ShowVictoryMenu()
    {
        if (victoryCanvas != null)
            victoryCanvas.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
        else
            SceneManager.LoadScene(mainMenuScene);
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}
