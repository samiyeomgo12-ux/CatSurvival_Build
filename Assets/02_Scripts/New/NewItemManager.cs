using UnityEngine;

public class NewItemManager : MonoBehaviour
{
    [SerializeField] private ItemData[] itemDatas;

    private int[] ItemLevels;
    private NewWeapon[] weapons;
    private Gear[] gears;

    private void Awake()
    {
        ItemLevels = new int[itemDatas.Length];
        weapons = new NewWeapon[itemDatas.Length];
        gears = new Gear[itemDatas.Length];
    }

    public int ItemCount => itemDatas.Length;
    public ItemData GetData(int idx) => itemDatas[idx];
    public int GetLevel(int idx) => ItemLevels[idx];
    public bool IsMaxLevel(int idx) => ItemLevels[idx] >= itemDatas[idx].damages.Length;

    public void InitWeapon(WeaponEnumId weaponKey) //무기 초기화(게임 시작 시 파사드에서 주입)
    {
        /*ItemData data = itemDatas[idx];
        GameObject weaponObj = new GameObject(data.itemName);
        weapons[idx] = weaponObj.AddComponent<NewWeapon>();
        weapons[idx].Init(data);*/

        int weaponIdx = FindWeaponIdx(weaponKey);
        if(weaponIdx == -1) return; //무기 못찾음        

        if (weapons[weaponIdx] == null)
        {
            ItemData data = itemDatas[weaponIdx];
            GameObject weaponObj = new GameObject(data.itemName);
            weapons[weaponIdx] = weaponObj.AddComponent<NewWeapon>();
            weapons[weaponIdx].Init(data);
           
           

        }
        /*    for(int i = 0; i < itemDatas.Length; i++)
            {
                if(itemDatas[i].weaponKey != weaponKey) continue;

                ItemData data = itemDatas[i];
                GameObject weaponObj = new GameObject(data.itemName);
                weapons[i] = weaponObj.AddComponent<NewWeapon>();
                weapons[i].Init(data);
                return;
            }*/
    }

    private int FindWeaponIdx(WeaponEnumId weaponKey)
    {
        for (int i = 0; i < itemDatas.Length; i++)
        {
            ItemData itemData = itemDatas[i];

            if (itemData.itemType != ItemData.ItemType.Melee && itemData.itemType != ItemData.ItemType.Range)
            {
                continue;
            }
            if (itemData.weaponKey == weaponKey)
            {
                return i;
            }
        }

        return -1;
    }
    public void SelectItem(int idx)
    {
        ItemData data = itemDatas[idx];
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (weapons[idx] == null)
                {
                    GameObject weaponObj = new GameObject(data.itemName);
                    weapons[idx] = weaponObj.AddComponent<NewWeapon>();
                    weapons[idx].Init(data);
                }
                weapons[idx].LevelUp(data.baseDamage,
                    data.counts[ItemLevels[idx]]);
                ItemLevels[idx]++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if (ItemLevels[idx] == 0)
                {
                    GameObject gearObj = new GameObject(data.itemName);
                    gears[idx] = gearObj.AddComponent<Gear>();
                    gears[idx].Init(data);
                }
                else
                {
                    gears[idx].LevelUp(data.damages[ItemLevels[idx]]);
                }
                ItemLevels[idx]++;
                break;

            case ItemData.ItemType.Heal:
                GameFacade.Instance.PlayerHealFull();
                break;
        }        
    }

}
