using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice;
    enum Achieve { UnLockBlackCat, UnLockPurpleCat }
    Achieve[] achives;
    WaitForSecondsRealtime wait;

    void Awake()
    {
        achives = (Achieve[])Enum.GetValues(typeof(Achieve));
        wait = new WaitForSecondsRealtime(5);
        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);

        foreach (Achieve achieve in achives)
        {
            PlayerPrefs.SetInt(achieve.ToString(), 0); //해금 되면 1, 아니면 0
        }
    }

    void Start()
    {
        UnlockCharacter();
    }

    void UnlockCharacter()
    {
        for (int index = 0; index < lockCharacter.Length; index++)
        {
            string achiveName = achives[index].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }

    void LateUpdate()
    {
        foreach (Achieve achieve in achives)
        {
            CheckAchive(achieve);
        }

    }
    void CheckAchive(Achieve achieve)
    {            
        
            bool isAchive = false;

            switch (achieve)
            {
                case Achieve.UnLockBlackCat:
                    //isAchive = GameManager.instance.kill >= 100; //100킬 이상이면 업적달성 -깜냥이 해금 조건 
                    isAchive = GameFacade.Instance.Kill >= 100; //100킬 이상이면 업적달성 -깜냥이 해금 조건 
                    break;
                case Achieve.UnLockPurpleCat:
                   // isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime; //끝까지 생존-보라냥 해금 조건 
                    isAchive = GameFacade.Instance.GameTime == GameFacade.Instance.MaxGameTime[0]; //끝까지 생존-보라냥 해금 조건 
                    break;
            }

            if(isAchive && PlayerPrefs.GetInt(achieve.ToString()) == 0)
            {
                PlayerPrefs.SetInt(achieve.ToString(), 1); //true
                PlayerPrefs.Save();

                for (int index = 0; index < uiNotice.transform.childCount; index++)
                {
                    bool isActive = index == (int)achieve;
                    uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
                }

                StartCoroutine(NoticeRoutine());
            }        
    }

    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true);

        GameFacade.Instance.PlaySfx(Sfx.LevelUp);
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);

        yield return wait;
        uiNotice.SetActive(false);
    }
  
}