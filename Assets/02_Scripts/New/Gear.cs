using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

public class Gear : MonoBehaviour
{
    public static event Action<Gear> OnApplyGear;
    public ItemData.ItemType type;

    public float rate; //레벨별 데이터 
    private NewWeapon nWeapon;
   
    public void Init(ItemData data)
    {
        //Basic Set
        name = "Gear" + data.itemId; 
        transform.parent = GameFacade.Instance.Player.transform;
        transform.localPosition = Vector3.zero;
        //Property set
        type = data.itemType;
        rate = data.damages[0];

       foreach(NewWeapon weapon in transform.parent.GetComponentsInChildren<NewWeapon>())
       {
           weapon.OnWeaponChanged += ApplyGear;
       }

       ApplyGear();
     
    }
    private void OnEnable()
    {
        if (transform.parent == null) return;
        foreach(NewWeapon w in transform.parent.GetComponentsInChildren<NewWeapon>())
        w.OnWeaponChanged += ApplyGear;
    }
    private void OnDisable()
    {
        if (transform.parent == null) return;
        foreach (NewWeapon w in transform.parent.GetComponentsInChildren<NewWeapon>())
            w.OnWeaponChanged -= ApplyGear;
    }
    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    void ApplyGear()
    {
     
        switch (type)
        {
            case ItemData.ItemType.Glove: //연사력 증가 
                RateUp();
                break;

            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
        OnApplyGear?.Invoke(this);
    }
    void RateUp() //연사력 -플레이어가 가진 모든 무기 
    {
        CharacterSO character = GameFacade.Instance.CurrentCharacter;

        NewWeapon[] weapons = transform.parent.GetComponentsInChildren<NewWeapon>();//찾아보기

        foreach (NewWeapon weapon in weapons)
        {
            switch (weapon.weaponType)
            {   
                case ItemData.ItemType.Melee: //근거리 
                    float speed = 150 * character.weaponSpeed;
                    //float speed = 150 * NewCharacter.WeaponSpeed;
                    weapon.rotateSpeed = speed + (speed * rate);
                    break;

                default:
                    speed = 0.5f * character.weaponRate;
                    //speed = 0.5f * NewCharacter.WeaponRate;
                    weapon.rotateSpeed = speed * (1f - rate);
                    break;
            }

        }
    }

    void SpeedUp()
    {
        //CharacterSO character = GameFacade.Instance.CurrentCharacter;

        //float speed = character.characterSpeed + character.characterSpeed * rate;
        //float speed = NewCharacter.Speed + NewCharacter.Speed * rate;
        //GameManager.instance.player.speed = speed + speed * rate;
        GameFacade.Instance.PlayerSpeedUp(rate);
    
    }
}
