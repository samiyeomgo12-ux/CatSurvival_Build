using UnityEngine;

public class NewHealItem : NewPickupItemBase
{
    [Header("힐 설정")]
    public float healAmount = 100f;

    protected override void OnCollected(Collider2D other)
    {
        GameFacade.Instance.PlayerHealFull();
        if (GameFacade.Instance.IsLive)
            GameFacade.Instance.PlaySfx(Sfx.OpenBox);
        StartCoroutine(DisableSoon());
    }
}
