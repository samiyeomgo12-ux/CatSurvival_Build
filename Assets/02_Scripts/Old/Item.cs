/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public WeaponManager weaponManager;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
      icon = GetComponentsInChildren<Image>()[1]; //두번째 이미지 가져오기
      icon.sprite = data.itemIcon;
      Text[] texts = GetComponentsInChildren<Text>();
      textLevel = texts[0];
      textName = texts[1];
      textDesc = texts[2];
      textName.text = data.itemName;

        //Get컴포넌트s의 순서는 계층 구조의 순서를 따라감
    }

    private void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);

        switch(data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;

            default:
                textDesc.text = string.Format(data.itemDesc); 
                break;
        }
    }

    public void OnClick()
    {
        switch(data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if(level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weaponManager = newWeapon.AddComponent<WeaponManager>();
                    weaponManager.Init(data);
                }
                else
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];

                    weaponManager.LevelUp(nextDamage, nextCount);
                }
                level++;
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:

                if(level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }
                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.playerHP = GameManager.instance.playerMaxHP;
                break;

        }


        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
*/