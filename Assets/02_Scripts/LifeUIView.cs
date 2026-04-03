using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LifeUIView : MonoBehaviour
{
    [SerializeField] Image[] heartImgs;
    [SerializeField] Sprite fullHearts;
    [SerializeField] Sprite lockHearts;
    [SerializeField] Text timerText;
    [SerializeField] GameObject[] chracterGroups;

    private void Start()
    {
        if (GameFacade.Instance == null) return;
        GameFacade.Instance.LifeManager.OnLifeChanged -= RefreshHearts;
        GameFacade.Instance.LifeManager.OnLifeChanged += RefreshHearts;
        GameFacade.Instance.LifeManager.OnTimerChanged -= RefreshTimer;
        GameFacade.Instance.LifeManager.OnTimerChanged += RefreshTimer;

        RefreshHearts(GameFacade.Instance.LifeManager.CurrentLife);
        RefreshTimer(GameFacade.Instance.LifeManager.GetRemainingTime());   
    }
    private void OnEnable()
    {
        if(GameFacade.Instance == null) return;
        GameFacade.Instance.LifeManager.OnLifeChanged += RefreshHearts;
        GameFacade.Instance.LifeManager.OnTimerChanged += RefreshTimer;

        RefreshHearts(GameFacade.Instance.LifeManager.CurrentLife);
        RefreshTimer(GameFacade.Instance.LifeManager.GetRemainingTime());
    }

    private void OnDisable()
    {
        if (GameFacade.Instance == null) return;
        GameFacade.Instance.LifeManager.OnLifeChanged -= RefreshHearts;
        GameFacade.Instance.LifeManager.OnTimerChanged -= RefreshTimer;
    }

    private void RefreshHearts(int currentLife)
    {
        for(int i = 0; i < heartImgs.Length; i++)
        {
            heartImgs[i].sprite = i < currentLife ? fullHearts : lockHearts;
        }

        for(int i = 0; i < chracterGroups.Length; i++)
        {
            if(currentLife <= 0) //생명이 하나도 없으면 캐릭터 선택 그룹 꺼지게 ?
            {
                var characterButtons = chracterGroups[i].GetComponentsInChildren<Button>();
                
                foreach(var button in characterButtons)
                {
                    button.interactable = false;
                }
            }
        }
        
    }

    private void RefreshTimer(TimeSpan remain)
    {
        if(GameFacade.Instance.LifeManager.CurrentLife >= GameFacade.Instance.LifeManager.MaxLife)
        {
            timerText.text = "";
            return;
        }

        timerText.text = $"{remain.Minutes:D2}:{remain.Seconds:D2}";
    }


}
