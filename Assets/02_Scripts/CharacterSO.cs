using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    [Header("캐릭터 선택")]
    public int characterID;
    public RuntimeAnimatorController characterController;
    public bool isRainbow;
    public bool isGold;
    public Color characterColor;
    public CharacterEnum key;
    public string characterName;
    public Sprite icon;
    public string cDescription;
    [Header("캐릭터 초기 설정")]
    public WeaponEnumId baseWeaponKey; //초기 시작 무기 
    public float characterSpeed;
    public float weaponSpeed;//원거리 무기 발사 속도
    public float weaponRate; //재장전 속도 
    public float damage;
    public int weaponCount; //무기 갯수
    [Header("캐릭터 해금 조건")]
  
    public CharacterUnlockCondition condition;

}
