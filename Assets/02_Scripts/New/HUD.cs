using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour
{
  public enum InfoType { Exp, Level, Kill, Time, HP }
  public InfoType type;

    Text myText;
    Slider mySlider;

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch(type)
        {
            case InfoType.Exp:
                //float curExp = GameManager.instance.exp;
                float curExp = GameFacade.Instance.Exp;
                //float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                float maxExp = GameFacade.Instance.NewPM.NextExp[Mathf.Min(GameFacade.Instance.NewPM.Level, GameFacade.Instance.NewPM.NextExp.Length-1)];
                mySlider.value = curExp / maxExp; 
                break;

            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameFacade.Instance.NewPM.Level); //{데이터의 인덱스 숫자, 데이터포멧}
                
                break;

            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameFacade.Instance.Kill); //{데이터의 인덱스 숫자, 데이터포멧}
                break;

            case InfoType.Time:
                //float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                float remainTime = GameFacade.Instance.MaxGameTime[GameFacade.Instance.CurrentMaxGameTimeIdx] - GameFacade.Instance.GameTime;
                int min = Mathf.FloorToInt(remainTime / 60); //분
                int sec = Mathf.FloorToInt(remainTime % 60); //초
                myText.text = string.Format("{0:D2}:{1:D2}",  min, sec);
                break;

            case InfoType.HP:
                //float curPlayerHP = GameManager.instance.playerHP;
                float curPlayerHP = GameFacade.Instance.PlayerHp;
                //float playerMaxHP = GameManager.instance.playerMaxHP;
                float playerMaxHP = GameFacade.Instance.NewPM.PlayerMaxHp;
                mySlider.value = curPlayerHP / playerMaxHP;
                break;

        }


    }
}
