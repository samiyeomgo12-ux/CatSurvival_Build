using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="Item", menuName = "Scriptble Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoe, Heal } //±ÙÁ¢, ¿ø°Å¸®, Àå°©, ½Å¹ß, Èú 

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;


    [Header("# Level Data")]
    public float baseDamage;
    //public int baseCount;
    public float[] damages;
    public float[] speeds;
    public int[] counts;


    [Header("# Weapon")]
    public WeaponEnumId weaponKey;
    public GameObject projecttile;
    public Sprite catFire;

}


