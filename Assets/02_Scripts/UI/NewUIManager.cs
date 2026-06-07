using UnityEngine;
using UnityEngine.UI;

public class NewUIManager : MonoBehaviour
{
    public Transform uiJoy;
    public CanvasGroup joyGroup;
    public NewResultView uiResult;
    public NewLevelUpView uiLevelUp;
    public PauseAndResume pauseAndResume;

    
    public void Win()
    {
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
    }

    public void Lose()
    {
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
    }

    public void JoyShowAndHide()
    {
        if(joyGroup.alpha == 0f) //투명하면
        {
            joyGroup.alpha = 1f; //보이게
        }
        else //보이면
        {
            joyGroup.alpha = 0f; //투명하게
        }
        
    }

    public void PauseAndResumeButtonShow()
    {
        pauseAndResume.PauseAndResumeButtonsShow();        
    }

    public void PauseAndResumeButtonHide()
    {
        pauseAndResume.PauseAndResumeButtonsHide();
    }

}
