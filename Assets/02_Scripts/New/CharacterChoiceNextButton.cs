using UnityEngine;
using UnityEngine.UI;
public class CharacterChoiceNextButton : MonoBehaviour
{


    [Header("페이지 넘기기")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private GameObject[] characterSet;
    [SerializeField] private int selectIdx;
    [SerializeField] private CharacterInfoView[] characterInfoView;

    private void Awake()
    {
        characterInfoView = GetComponentsInChildren<CharacterInfoView>();
        selectIdx = 0;
        UpdateView();
        //characterSet[selectIdx].SetActive(true);
    }

    private void UpdateView() //선택된 페이지만 보이게
    {
        for (int i = 0; i < characterSet.Length; i++)
        {
            characterSet[i].SetActive(i == selectIdx);
        }      

        foreach(var c in characterInfoView)
        {
            c.UpdateCharacterChoiceView();
        }
    }
    public void RightSelect()
    {
        selectIdx++;
        selectIdx = selectIdx % characterSet.Length;      
        
        for(int i = 0; i < characterSet.Length; i++)
        {
            characterSet[i].SetActive(i == selectIdx);//선택패널만 활성화
        }      
    }

    public void LeftSelect()
    {       
        selectIdx--;
        if(selectIdx < 0)
        {
            selectIdx = characterSet.Length - 1;
        }

        for (int i = 0; i < characterSet.Length; i++)
        {
            characterSet[i].SetActive(i == selectIdx);
        }

        
    }
}
