using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using Unity.VisualScripting;

public class NewObjectPoolManager : MonoBehaviour
{
    public static NewObjectPoolManager Instance;
    [SerializeField] PoolItemDataBase poolItemDatabase;

    private Dictionary<EnemyEnumId, NewPool> enemyPools = new Dictionary<EnemyEnumId, NewPool>();
    private Dictionary<WeaponEnumId, NewPool> weaponPools = new Dictionary<WeaponEnumId, NewPool>();
    private Dictionary<ItemEnumId, NewPool> itemPools = new Dictionary<ItemEnumId, NewPool>();

    private const int maxSize = 100;
    private const int initSize = 50;

    private GameObject prefab;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        InitPool();
    }

    void InitPool()
    {
        foreach(var item in poolItemDatabase.items)
        {
            NewPool pool;

            if(item.type == PoolObjType.Enemy)
            {
                if(!enemyPools.ContainsKey(item.enemyKey))
                {
                    pool = new NewPool(item.prefab, item.initSize);
                    enemyPools.Add(item.enemyKey, pool);
                }
            }
            else if(item.type == PoolObjType.Weapon)
            {
                if(!weaponPools.ContainsKey(item.weaponKey))
                {
                    pool = new NewPool(item.prefab, item.initSize);
                    weaponPools.Add(item.weaponKey, pool);
                }
            }
            else
            {
                if(!itemPools.ContainsKey(item.itemkey))
                {
                    pool = new NewPool(item.prefab, item.initSize);
                    itemPools.Add(item.itemkey, pool);
                }
            }
        }
    }

    public GameObject Get(EnemyEnumId enemyKey)
    {
        if(!enemyPools.ContainsKey(enemyKey))
        {
            return null;
        }

        var obj = enemyPools[enemyKey].Get();
        var enemy = obj.GetComponent<NewEnemy>();
        //enemy.SetUp(enemyData);
        return obj;

    }

    public GameObject Get(WeaponEnumId weaponKey)
    {
        if(!weaponPools.ContainsKey(weaponKey))
        {
            return null;
        }

        return weaponPools[weaponKey].Get();
    }

    public GameObject Get(ItemEnumId itemKey)
    {
        if(!itemPools.ContainsKey(itemKey))
        {
            return null;
        }
        return itemPools[itemKey].Get();    
    }

    public void Return(EnemyEnumId enemyKey, GameObject obj)
    {
        if(!enemyPools.ContainsKey(enemyKey))
        {
            Debug.Log($"키 없음 {enemyKey}");
            return;
        }

        enemyPools[enemyKey].Return(obj); //각 풀로 오브젝트 반환하기
    }

    public void Return(WeaponEnumId weaponKey, GameObject obj)
    {
        if(!weaponPools.ContainsKey(weaponKey))
        {
            Debug.Log($"키 없음 {weaponKey}");
            return;
        }
        weaponPools[weaponKey].Return(obj);
    }
    public void Return(ItemEnumId itemKey, GameObject obj)
    {
        if(!itemPools.ContainsKey(itemKey))
        {
            Debug.Log($"키 없음 {itemKey}");
            return;
        }

        itemPools[itemKey].Return(obj);
    }
}
