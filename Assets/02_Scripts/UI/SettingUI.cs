using UnityEngine;
using UnityEngine.UI;
public class SettingUI : MonoBehaviour
{
    [SerializeField] private Button settingButton;
    [SerializeField] GameObject settingView;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;

    public void SetButtonToggle()
    {
        if (settingView.activeSelf) //활성화 되어 있으면
        {
            settingView.SetActive(false); //끄기
            //GameFacade.Instance.GameResume();
        }
        else
        {
            settingView.SetActive(true);//켜기
            //GameFacade.Instance.GameStop();
        }
    }

    public void CloseSettionView()
    {
        settingView.SetActive(false);
    }

    public void SetBGMVolume()
    {
        float v = bgmVolumeSlider.value;
        GameFacade.Instance.SetBGMVolume(v);
    }
    public void SetSFXVolume()
    {
        float v = sfxVolumeSlider.value;
        GameFacade.Instance.SetSFXVolume(v);
    }
}
