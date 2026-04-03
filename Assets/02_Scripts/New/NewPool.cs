using UnityEngine;
using System.Collections.Generic;
public class NewPool
{
    private GameObject prefab;
    private Queue<GameObject> poolQueue;

    public NewPool(GameObject prefab, int initSize)
    {
        this.prefab = prefab;
        poolQueue = new Queue<GameObject>();

        for (int i = 0; i < initSize; i++)
        {
            var obj = GameObject.Instantiate(prefab);
            poolQueue.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    public GameObject Get()
    {
        if(poolQueue.Count > 0)
        {
            var obj = poolQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        var newobj = GameObject.Instantiate(prefab);
        newobj.SetActive(true);
        return newobj;
    }
    public void Return(GameObject obj)
    {
        poolQueue.Enqueue(obj);
        obj.SetActive(false);
    }
}
