using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NewGameManager : MonoBehaviour
{
    [Header("게임 진행")]
    public bool isLive;

    public GameObject enemyCleaner;
    public event Action OnGameVictiory;
    public event Action OnGameOver;
    public event Action OnStop;
    public event Action OnResume;

    private void Awake()
    {
        Application.targetFrameRate = 60;        
    }

  
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0f;
        OnStop?.Invoke();

    }

    public void GameResume()
    {
        isLive = true;
        Time.timeScale = 1f;
        OnResume?.Invoke();
    }
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        OnGameVictiory?.Invoke();
        Stop();
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }
    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);
        OnGameOver?.Invoke();
        Stop();
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
    public void GameQuit()
    {
        Application.Quit();
    }
   
}
