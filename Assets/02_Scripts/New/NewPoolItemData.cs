using Unity.VisualScripting;
using UnityEngine;

public enum PoolObjType
{
    Enemy, 
    Weapon, 
    Item
}
public struct PoolObjInfo
{
    public PoolObjType type { get; }
    public EnemyEnumId? enemyKey; 
    public WeaponEnumId? weaponKey;
    public ItemEnumId? itemKey;
    public int initalSize { get; }
    public int maxSize { get; }
    public GameObject prefab;

    public PoolObjInfo(EnemyEnumId enemyKey, int size, int maxSize, GameObject prefab)
    {
        this.type = PoolObjType.Enemy;
        this.enemyKey = enemyKey;
        this.weaponKey = null; 
        this.itemKey = null;
        this.initalSize = size;
        this.maxSize = maxSize;
        this.prefab = prefab;
    }
    public PoolObjInfo(WeaponEnumId weaponKey, int size, int maxSize, GameObject prefab)
    {
        this.type = PoolObjType.Weapon;
        this.weaponKey = weaponKey;
        this.enemyKey = null;
        this.itemKey= null;
        this.initalSize = size;
        this.maxSize = maxSize;
        this.prefab = prefab;
    }
    public PoolObjInfo(ItemEnumId itemKey, int size, int maxSize, GameObject prefab)
    {
        this.type = PoolObjType.Item;
        this.itemKey = itemKey;
        this.weaponKey = null; 
        this.enemyKey= null;
        this.initalSize = size;
        this.maxSize = maxSize;
        this.prefab = prefab;
    }   
}


[CreateAssetMenu(fileName = "NewPoolItemData", menuName = "Scriptable Objects/NewPoolItemData")]
public class NewPoolItemData : ScriptableObject
{
    public PoolObjType type;
    public GameObject prefab;

    public EnemyEnumId enemyKey; 
    public WeaponEnumId weaponKey;
    public ItemEnumId itemkey;
    public int initSize;
    public int maxSize;

    public PoolObjInfo PoolInfo()
    {
        if(type == PoolObjType.Enemy)
        {
            return new PoolObjInfo(enemyKey, initSize, maxSize, prefab);
        }
        else if(type == PoolObjType.Weapon)
        {
            return new PoolObjInfo(weaponKey, initSize, maxSize, prefab);
        }
        else //æ∆¿Ã≈€ 
        {
            return new PoolObjInfo(itemkey, initSize, maxSize, prefab);
        }
    }
}
