/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true); //자식들에게 있는 컴포넌트 가져올 것~. 비활성화 되어 있는 애들 가져와야 하니까 true

    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop(); //레벨업 아이템 고를 때 시간 잠시 멈춰둠 
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume(); //레벨업 아이템 고르기 끝나면 시간 재개 
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        //1.모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        //2. 그 중에서 랜덤하게 3개의 아이템만 활성화 하기 
        int[] ran = new int[3];
        while (true)
        {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            if (ran[0]!=ran[1] && ran[1]!=ran[2] && ran[1]!=ran[2])
                break;
        }

        for (int index = 0; index < ran.Length; index++)
        {
            Item ranItem = items[ran[index]];

            //3. 만렙 아이템의 경우는 소비 아이템으로 대체하기 
            if(ranItem.level == ranItem.data.damages.Length)
            {
                items[Random.Range(4, 7)].gameObject.SetActive(true);
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }
               
        }

    }
}

*/