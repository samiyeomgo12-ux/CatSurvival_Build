using UnityEngine;

public class NewLevelUpView : MonoBehaviour
{
    private RectTransform rect;
    private NewItem[] items;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<NewItem>(true);
    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameFacade.Instance.GameStop(); // 레벨업 아이템 고를 때 잠시 멈추기 
        GameFacade.Instance.PlaySfx(Sfx.LevelUp);
        GameFacade.Instance.EffectBgm(true);
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameFacade.Instance.GameResume(); //게임 재 시작 
        GameFacade.Instance.PlaySfx(Sfx.Select);
        GameFacade.Instance.EffectBgm(false);
        
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    private void Next()
    {
        //모든 아이템 비활성화
        foreach (NewItem item in items)
        {
            item.gameObject.SetActive(false);
        }

        int[] ran = new int[3];

        while (true)
        {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                break;
        }

        for (int index = 0; index < ran.Length; index++)
        {
            int itemIdx = ran[index];
            NewItem ranItem = items[itemIdx];
       
            if (GameFacade.Instance.IsItemMaxLevel(itemIdx))
            {
                //아이템이 만랩이라면, 소비형 아이템으로 
               //items[Random.Range(2, 5)].gameObject.SetActive(true);
            }
            else
            {
                //ranItem.gameObject.SetActive(true);
                ranItem.gameObject.SetActive(true);
                ranItem.Setup(GameFacade.Instance.GetItemData(itemIdx),
                    GameFacade.Instance.GetItemLevel(itemIdx),
                    itemIdx);
               
            }
        }

        /*
                foreach (NewItem item in items)
                {
                    item.Setup(item.data, GameFacade.Instance.GetItemLevel(item.data.itemId), GameFacade.Instance.)
                }*/
    }
}
