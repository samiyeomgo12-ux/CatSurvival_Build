using UnityEngine;

public class NewMagnetItem : NewPickupItemBase
{
    [Header("자석 설정")]
    public float magnetRadius = 10f;
    public float magnetDuration = 5f;

    protected override void OnCollected(Collider2D other)
    {
        GameFacade.Instance.ActivateMagnet(magnetRadius, magnetDuration);
        if (GameFacade.Instance.IsLive)
            GameFacade.Instance.PlaySfx(Sfx.OpenBox);
        StartCoroutine(DisableSoon());
    }
}
