using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : Singleton<RespawnManager> 
{
    [Header("Tentativi")]
    [SerializeField] private int maxTry = 3;
    private int leftTry;

    public int LeftTry => leftTry;
    public int MaxTry => maxTry;

    [Header("Respawn Settings")]
    [SerializeField] private Transform puntoRespawn;
    [SerializeField] private float respawnDelay = 2f;

    private GameObject player;

    protected override bool ShouldBeDestroyOnLoad() => false; 

    protected override void Awake()
    {
        base.Awake();
        leftTry = maxTry;
    }

    private void Start()
    {
        FindPlayer();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy(); 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
        FindObjectOfType<UILives>()?.UpdateLives();
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ResetTries()
    {
        leftTry = maxTry;
    }

    public void PlayerDied()
    {
        leftTry--;

        FindObjectOfType<UILives>()?.UpdateLives();

        if (leftTry > 0)
        {
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            GameOver();
        }
    }

    private IEnumerator RespawnRoutine()
    {
        if (player == null) yield break;

        bool done = false;
        ScreenFader.Instance.FadeOut(() => done = true);

        while (!done) yield return null;

        player.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);

        var life = player.GetComponent<LifeController>();
        if (life != null) life.SetHp(life.GetMaxHp());

        player.transform.position = puntoRespawn.position;
        player.SetActive(true);

        done = false;
        ScreenFader.Instance.FadeIn(() => done = true);
        while (!done) yield return null;
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
        ResetTries();
        ScreenFader.Instance.FadeOut(() =>
        {
            SceneManager.LoadScene("StartMenu");
            ScreenFader.Instance.FadeIn();
        });
    }
}

