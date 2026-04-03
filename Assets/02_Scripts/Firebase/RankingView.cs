using Firebase.AppCheck;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class NameUtil
{
    public static string GetDisplayName(string nickname, string uid)
    {
        if(!string.IsNullOrWhiteSpace(nickname))return nickname;

        if (string.IsNullOrEmpty(uid)) return "Unknown";

        return $"Guest_{uid.Substring(0, Mathf.Min(8, uid.Length))}";
    }
}
public class RankingView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text rankingText;
    [SerializeField] private Text nicknameText;
    [SerializeField] private Text killText;

    [Header("Rank Icon UI")]
    [SerializeField] private Image rankIconImage;
    [SerializeField] private Sprite firstPlaceSprite;
    [SerializeField] private Sprite secondPlaceSprite;
    [SerializeField] private Sprite thirdPlaceSprite;
    [SerializeField] private Sprite defaultSprite; //4µÓ ¿Ã«œ

    public void Bind(int rank, string nickname, string uid, int bestKills)
    {
        rankingText.text = rank.ToString();
        nicknameText.text = NameUtil.GetDisplayName(nickname, uid);
        killText.text = bestKills.ToString();

        Sprite selected = null; 
        
        switch(rank)
        {
            case 1: selected = firstPlaceSprite; break;
            case 2: selected = secondPlaceSprite; break;
            case 3: selected = thirdPlaceSprite; break;
            default: selected = defaultSprite; break;
        }

        if (rankIconImage == null) return;

        if (selected != null)
        {
            rankIconImage.gameObject.SetActive(true);
            rankIconImage.sprite = selected;
        }
        else
        {
            rankIconImage.gameObject.SetActive(false);
        }


    }

   
}

