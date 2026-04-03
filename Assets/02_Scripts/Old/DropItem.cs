using System.Collections;
using UnityEngine;

public class DropItem : NewPickupItemBase
{
    public enum ItemType { Heal, Magnet }

    [Header("Type")] public ItemType type = ItemType.Heal;
    [Header("Heal")] public float healAmount = 100f;   // 풀힐
    [Header("Magnet")] public float magnetRadius = 10f; // 자석 반경
    public float magnetDuration = 5f;                   // 자석 유지 시간   
    public bool playSfx = true;
     
    public void InitHeal(float amount)
    {
        type = ItemType.Heal;
        healAmount = amount;
        ResetPickupState();
    }
    
    public void InitMagnet(float radius, float duration)
    {
        type = ItemType.Magnet;
        magnetRadius = radius;
        magnetDuration = duration;
        ResetPickupState();
    }

    protected override void OnCollected(Collider2D other)
    {
        switch(type)
        {
            case ItemType.Heal:
                GameFacade.Instance.PlayerHealFull();
                if (playSfx && GameFacade.Instance.IsLive)
                    GameFacade.Instance.PlaySfx(Sfx.OpenBox);
                break;
            case ItemType.Magnet:
            //자석 
                GameFacade.Instance.PlaySfx(Sfx.OpenBox);
                break;
        }

        StartCoroutine(DisableSoon());
    }
}
