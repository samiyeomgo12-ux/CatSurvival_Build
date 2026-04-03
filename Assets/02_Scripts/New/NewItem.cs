using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewItem : MonoBehaviour
{
    public ItemData data; //SO
    [System.NonSerialized]public int level;
    [System.NonSerialized]public NewWeapon nWeapon;
    [System.NonSerialized]public Gear gear;
    [System.NonSerialized] public int itemManagerIndex;

    [SerializeField] private Image icon;
    [SerializeField] private Text textLevel;
    [SerializeField] private Text textName;
    [SerializeField] private Text textDesc;


    public void OnClick()
    {
        GameFacade.Instance.SelectItem(itemManagerIndex);
    }

    public void Setup(ItemData data, int level, int managerIdx)//레벨업 시 랜덤으로 3개 골라서 세팅
    {
        this.data = data;
        itemManagerIndex = managerIdx;
        this.level = level;
        icon.sprite = data.itemIcon;
        textName.text = data.itemName;
        textLevel.text = "Lv." + (level + 1);

        switch (data.itemType)
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
                textDesc.text = data.itemDesc;
                break;
        }
    }

    /*private void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);

        switch (data.itemType)
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
    }*/




   /* public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (nWeapon == null) //무기 존재 여부 
                {
                    GameObject newWeapon = new GameObject();
                    nWeapon = newWeapon.AddComponent<NewWeapon>();
                    nWeapon.Init(data);
                }
                *//* else
                 {
                     float nextDamage = data.baseDamage + data.baseDamage * data.damages[level];
                     int nextCount = data.counts[level];
                     nWeapon.LevelUp(nextDamage, nextCount);
                 }*//*

                float nextDamage = data.baseDamage + data.baseDamage * data.damages[level];
                int nextCount = data.counts[level];
                nWeapon.LevelUp(nextDamage, nextCount);
                level++;
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:

                if (level == 0)
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
                GameFacade.Instance.PlayerHealFull();
                break;

        }


        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }*/
}
