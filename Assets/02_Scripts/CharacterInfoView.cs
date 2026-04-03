using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoView : MonoBehaviour
{
    [Header("캐릭터 정보 설정")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text characterDes;
    [SerializeField] CharacterSO characterSO;

    private void Awake()
    {
        /*Image[] images = GetComponentsInChildren<Image>();
        iconImage = images[1];
       // iconImage = GetComponent<Image>();
        Text[] texts = GetComponentsInChildren<Text>();

        nameText = texts[0];
        characterDes = texts[1];*/
    }

    private void OnEnable()
    {
        UpdateCharacterChoiceView();
    }

    public void UpdateCharacterChoiceView()
    {
        iconImage.sprite = characterSO.icon;
        iconImage.color = characterSO.characterColor;
        nameText.text = characterSO.characterName;
        characterDes.text = characterSO.cDescription;

    }

}
