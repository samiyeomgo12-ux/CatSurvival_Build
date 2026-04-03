using UnityEngine;

// 플레이어에 부착. 자석 아이템 효과 중 주변 코인/아이템을 플레이어에게 당겨옴.
public class MagnetPuller : MonoBehaviour
{
    [SerializeField] private float magnetSpeed = 5f;
    [SerializeField] private LayerMask itemMask;

    private void Update()
    {
        if (!GameFacade.Instance.IsMagnetActive()) return;

        Collider2D[] items = Physics2D.OverlapCircleAll(
            transform.position, GameFacade.Instance.MagnetRadius, itemMask);

        float step = magnetSpeed * Time.deltaTime;
        foreach (var item in items)
        {
            if (!item.gameObject.activeInHierarchy) continue;
            item.transform.position = Vector3.MoveTowards(
                item.transform.position, transform.position, step);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (GameFacade.Instance == null || !GameFacade.Instance.IsMagnetActive()) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, GameFacade.Instance.MagnetRadius);
    }
}
