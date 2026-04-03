using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class NewAchiveManager : MonoBehaviour
{
    public static NewAchiveManager Instance { get; private set; }
    [System.Serializable]
    public class UnlockableCharacter
    {
        public CharacterSO characterSO;
        public GameObject lockObject;
        public GameObject unlockObject;
        public Text unlockText;
    }

    [Header("해금 캐릭터 목록")]
    public UnlockableCharacter[] unlockableCharacters;

    [Header("해금 알림 팝업 UI")]
    public GameObject uiNoticeObj;
    public Image uiCharacterIcon;
    public Text noticeText;
    public bool isNotice;
    private WaitForSecondsRealtime wait;

    private void Awake()
    {
        wait = new WaitForSecondsRealtime(5);
        InitPlayerPrefs();
        RefreshUnlockUI();

        if (Instance == null) Instance = this;
    }
    void Update()
    {
        foreach(var c in unlockableCharacters)
        {
            CheackAndUnlock(c);
        }
    }

    public void UnlockAllCharactersByCheat()
    {
        foreach(var character in unlockableCharacters)
        {
            string key = GetSaveKey(character);

            PlayerPrefs.SetInt(key, 1); //해금 
        }

        PlayerPrefs.Save();
        RefreshUnlockUI();

        if(noticeText != null)
        {
            noticeText.text = "치트 성공. 모든 캐릭터 해금";
        }


    }
    private void InitPlayerPrefs()
    {
        if(!PlayerPrefs.HasKey("MyData"))
        {
            PlayerPrefs.SetInt("MyData", 1); //해금 되면 1, 아니면 0

            foreach(var character in unlockableCharacters)
            {
                string key = GetSaveKey(character);
                bool isDefaultUnlocked = character.characterSO.condition.unlockType == UnlockType.None;
                PlayerPrefs.SetInt(key, isDefaultUnlocked ? 1 : 0);
            }

            PlayerPrefs.Save();
        }
    }
    private string GetSaveKey(UnlockableCharacter unlockCharacter)
    {
        return unlockCharacter.characterSO.characterName; //캐릭터 이름을 키로 가져오기
    }

    void CheackAndUnlock(UnlockableCharacter unlockCharacter)
    {
        CharacterSO character = unlockCharacter.characterSO;
        string key = GetSaveKey(unlockCharacter);

        if (PlayerPrefs.GetInt(key) == 1) return; //이미 해금 되어 있으면 스킵하기 

        if (character.condition.CheckUnlockCondition()) //해금 조건 확인 
        {
            PlayerPrefs.SetInt(key, 1); //해금 저장 
            PlayerPrefs.Save();

            RefreshUnlockUI();

            if(character.condition.unlockType != UnlockType.None) 
            ShowNotice(unlockCharacter);
        }

    }
    private void RefreshUnlockUI()
    {
        foreach(var character in unlockableCharacters)
        {
            bool isUnlocked = PlayerPrefs.GetInt(GetSaveKey(character)) == 1; //해금 되면 true

            character.lockObject.SetActive(!isUnlocked);
            character.unlockObject.SetActive(isUnlocked);

            if(isUnlocked)
            {
                character.unlockText.text = character.characterSO.condition.GetUnlockDescription();//해금 조건 설명
            }
        }
    }

    void ShowNotice(UnlockableCharacter unlockCharacter)
    {
        if (isNotice) return;
        uiCharacterIcon.sprite = unlockCharacter.characterSO.icon;
        noticeText.text = unlockCharacter.characterSO.characterName + "가 합류했습니다";

        StartCoroutine(NoticeRoutine());
    }

    IEnumerator NoticeRoutine()
    {
        isNotice = true; 
        
        uiNoticeObj.SetActive(true);
        GameFacade.Instance.PlaySfx(Sfx.LevelUp);

        yield return wait; //5초 기다리기

        uiNoticeObj.SetActive(false);
        isNotice = false;
    }
}

