using System.Net.NetworkInformation;
using UnityEngine;

public class PauseAndResume : MonoBehaviour
{
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject pauseAndResumeButton;

    public void Pause()
    {
        GameFacade.Instance.GameStop();
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);
    }
    public void Resume()
    {
        GameFacade.Instance.GameResume();
        pauseButton.SetActive(true);
        resumeButton.SetActive(false);
    }

    public void PauseAndResumeButtonsShow()
    {
        pauseAndResumeButton.SetActive(true);
              
    }

    public void PauseAndResumeButtonsHide()
    {
        pauseAndResumeButton.SetActive(false);
    }
}
