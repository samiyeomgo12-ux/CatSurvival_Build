using System;
using System.Net.NetworkInformation;
using UnityEngine;

public class NewWeapon : MonoBehaviour
{
    public int id;
    public WeaponEnumId weaponKey;
    public ItemData.ItemType weaponType;
    public float damage;
    public int count;
    public float rotateSpeed;
    public float timer; 
    NewPlayer player;

    public event Action OnWeaponChanged;

    public void Init(ItemData data)
    {
        if(player == null)
        {
            player = GameFacade.Instance.Player;           
        }
        
        CharacterSO character = GameFacade.Instance.CurrentCharacter;

        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = GameFacade.Instance.PlayerId;

        damage = data.baseDamage * character.damage;
        count =  character.weaponCount;    
       //  damage = data.baseDamage * NewCharacter.Damage;
       // count = data.baseCount +  NewCharacter.Count;
        weaponKey = data.weaponKey;
        Debug.Log($"NewWeapon의 {count}");
        weaponType = data.itemType;

        switch(weaponType)
        {
            case ItemData.ItemType.Melee:
                rotateSpeed = 150 * character.weaponSpeed;
                //rotateSpeed = 150 * NewCharacter.WeaponSpeed;
                Batch();
                break;
            default:
                Fire();
                rotateSpeed = 0.3f* character.weaponRate;
                //rotateSpeed = 0.3f * NewCharacter.WeaponSpeed;
                break;
        }

        OnWeaponChanged?.Invoke();
    }
    private void Awake()
    {
        player = GameFacade.Instance?.Player;
    }

    private void Update()
    {
        if (!GameFacade.Instance.IsLive) return;

        switch(weaponType)
        {
            case ItemData.ItemType.Melee:
                transform.Rotate(Vector3.back * rotateSpeed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;
                if(timer > rotateSpeed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }

    }

    void Batch()
    {
        for (int idx = 0; idx < count; idx++)
        {
            Transform catAttack;

            if (idx < transform.childCount)
            {
                catAttack = transform.GetChild(idx);
            }
            else
            {
                catAttack = GameFacade.Instance.Get<WeaponEnumId>(weaponKey).transform;
            }

            catAttack.parent = transform;
            catAttack.localPosition = Vector3.zero;
            catAttack.localRotation = Quaternion.identity;

            float angle = 360f * idx / count;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.up;

            catAttack.localPosition = dir * 1.5f;
            catAttack.localRotation = Quaternion.Euler(0, 0, angle);

            catAttack.GetComponent<CatAtack>().Init(damage, -100, Vector3.zero);

        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        float offsetDistance = 1f;
        Vector3 spawnPos = transform.position + dir * offsetDistance;
        Transform fireArrow = GameFacade.Instance.Get<WeaponEnumId>(weaponKey).transform;
        fireArrow.position = spawnPos;
        fireArrow.rotation = Quaternion.FromToRotation(Vector3.right, dir);

        fireArrow.GetComponent<CatAtack>().Init(damage, count, dir);
        GameFacade.Instance.PlaySfx(Sfx.Range);
    }

    public void LevelUp(float damage, int count)
    {
        CharacterSO character = GameFacade.Instance.CurrentCharacter;
        this.damage = damage * character.damage;
        this.count += count;

        if (weaponType == ItemData.ItemType.Melee)
            Batch(); //회전 무기
        OnWeaponChanged?.Invoke();
    }
}
