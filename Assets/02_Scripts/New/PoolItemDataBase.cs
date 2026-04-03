using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PoolItemDataBase", menuName = "Scriptable Objects/PoolItemDataBase")]
public class PoolItemDataBase : ScriptableObject
{
    public List<NewPoolItemData> items;
}
