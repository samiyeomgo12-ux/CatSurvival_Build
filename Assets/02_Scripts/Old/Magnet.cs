/*using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] float magnetRadius = 3f;  // 끌어당기는 범위(에디터 조절)
    [SerializeField] float magnetSpeed = 3f;  // 끌려오는 속도
    [SerializeField] LayerMask coinMask;       // Coin 전용 레이어마스크

    void Awake()
    {
        // Coin 레이어가 10번이면 이렇게 한 번만 설정하면 됨
        coinMask = 1 << 10; // == 1 << LayerMask.NameToLayer("Coin")
    }

    void Update()
    {
        // 자석 아이템을 먹어 전역 자석이 켜졌을 때만 작동
        if (GameManager.instance == null || !GameManager.instance.IsMagnetActive())
            return;

        // 반경 내의 코인(coinMask)만 탐색
        Collider2D[] coins = Physics2D.OverlapCircleAll(transform.position, magnetRadius, coinMask);

        float step = magnetSpeed * Time.deltaTime;
        for (int i = 0; i < coins.Length; i++)
        {
            Transform t = coins[i].transform; //
            // 비활성/파괴 체크
            if (t == null || !t.gameObject.activeInHierarchy) continue;

            t.position = Vector3.MoveTowards(t.position, transform.position, step);
        }
    }

    // 디버그용: 선택 시 반경 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
*/