using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

#region ver.3

public class Reposition : MonoBehaviour
{
    [SerializeField] private float checkDistance = 25f;      // 체크할 거리
    [SerializeField] private float repositionDistance = 40f; // 재배치 거리(타일 청크 크기와 동일 권장)
    [SerializeField] private float enemyResawnDistance = 20; // 적 리스폰 거리
    [SerializeField] private float enemyCheckDistance = 20f; // 적 체크 거리 

    Collider2D coll;
    Transform player;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    void Start()
    {
        // player = GameFacade.Instance.Player.transform;
        //player = GameManager.instance.player.transform;
    }

    // [FIX] 타일/적 재배치를 LateUpdate에서 수행하여
    //       플레이어 이동이 완료된 뒤 보정되도록 함(프레임 튐/지연 감소)
    void LateUpdate()
    {
        if (!player)
        {
            var p = GameFacade.Instance?.Player;
            if(p == null) return;
            player = p.transform;
        }

        if (transform.CompareTag("Ground"))
        {
            CheckAndRepositionTile();
        }
        else if (transform.CompareTag("Enemy"))
        {
            CheckAndRepositionEnemy();
        }
    }

    void CheckAndRepositionTile()
    {
        if (!player) return;

        Vector3 playerPos = player.position;
        Vector3 tilePos = transform.position;
        bool moved = false;

        while (playerPos.x - tilePos.x > checkDistance) 
        { 
            tilePos.x += repositionDistance; 
            moved = true; 
        }
        while (playerPos.x - tilePos.x < -checkDistance) 
        { 
            tilePos.x -= repositionDistance; 
            moved = true; 
        }
        while (playerPos.y - tilePos.y > checkDistance) 
        { 
            tilePos.y += repositionDistance; 
            moved = true; 
        }
        while (playerPos.y - tilePos.y < -checkDistance) 
        { 
            tilePos.y -= repositionDistance;
            moved = true; 
        }

        if (moved)
        {
            transform.position = tilePos;
        }
    }

    void CheckAndRepositionEnemy()
    {
        if (!coll || !coll.enabled) return;

        Vector3 playerPos = player.position;
        Vector3 enemyPos = transform.position;

        float distance = Vector3.Distance(playerPos, enemyPos);

        if (distance > enemyCheckDistance)
        {
            RepositionEnemy();
        }
    }

    void RepositionEnemy()
    {
        Vector3 playerPos = player.position;

        // [FIX] 입력 벡터 타입 정렬: inputVec이 Vector2인 경우를 안전하게 처리하고 Vector3로 승격
        //Vector2 input2 = GameManager.instance.player.inputVec;                 // ← Vector2 가정(대부분 이동 입력)
        Vector2 input2 = GameFacade.Instance.Player.inputVec;                 // ← Vector2 가정(대부분 이동 입력)
        Vector3 spawnDirection;

        if (input2 != Vector2.zero)
        {
            spawnDirection = new Vector3(input2.x, input2.y, 0f).normalized;   // [FIX] Vector2 → Vector3 변환
            float randomAngle = Random.Range(-45f, 45f);                       // [FIX] 오타 수정: randomAnle → randomAngle
            spawnDirection = Quaternion.Euler(0, 0, randomAngle) * spawnDirection;
        }
        else
        {
            float randomAngle360 = Random.Range(0f, 360f);                     // [FIX] 오타 수정: randonAngle → randomAngle360
            spawnDirection = new Vector3(Mathf.Cos(randomAngle360 * Mathf.Deg2Rad),
                                         Mathf.Sin(randomAngle360 * Mathf.Deg2Rad), 0f);
        }

        Vector3 spawnOffset = spawnDirection * enemyCheckDistance;
        spawnOffset += new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);

        Vector3 newPos = playerPos + spawnOffset;
        transform.position = newPos;
        // Debug.Log($"Repositioned Enemy {gameObject.name} to {newPos}");
    }

    // 기존 OnTriggerExit은 백업으로 유지(원하면 비활성화해도 됨)
    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        // Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 playerPos = GameFacade.Instance.Player.transform.position;  
        Vector3 tileMapPos = transform.position;

        float diffX = Mathf.Abs(playerPos.x - tileMapPos.x);
        float diffY = Mathf.Abs(playerPos.y - tileMapPos.y);
        float dirX = Mathf.Sign(playerPos.x - tileMapPos.x);
        float dirY = Mathf.Sign(playerPos.y - tileMapPos.y);

        Vector3 newPos = tileMapPos;

        // [FIX] 입력 벡터 타입 정렬 (Vector2 가정)
        //Vector2 playerDir2 = GameManager.instance.player.inputVec; // ← Vector2
        Vector2 playerDir2 = GameFacade.Instance.Player.inputVec;
        Vector3 playerDir = new Vector3(playerDir2.x, playerDir2.y, 0f); // [FIX] Vector2 → Vector3

        switch (transform.tag)
        {
            case "Ground":
                // [FIX] 매직 넘버 40f 대신 필드값 사용으로 일관성 확보
                if (Mathf.Abs(diffX - diffY) <= 5f)
                {
                    newPos.x += dirX * repositionDistance;
                    newPos.y += dirY * repositionDistance;
                }
                else
                {
                    if (diffX > diffY)
                        newPos.x += dirX * repositionDistance;
                    else
                        newPos.y += dirY * repositionDistance;
                }

                transform.position = newPos;
                break;

            case "Enemy":
                if (coll && coll.enabled)
                {
                    // [FIX] Vector2 입력을 Vector3로 변환하여 사용
                    transform.Translate(playerDir * enemyResawnDistance
                                        + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
                }
                break;
        }
    }
}

#endregion

#region ver2.
/*public class Reposition : MonoBehaviour
{
    [SerializeField] private float checkDistance = 25f; // 체크할 거리
    [SerializeField] private float repositionDistance = 40f; // 재배치 거리
    [SerializeField] private float enemyResawnDistance = 20; // 적 리스폰 거리
    [SerializeField] private float enemyCheckDistance = 13f; //적 체크 거리 

    Collider2D coll;

    private Transform player;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
    }
    private void Start()
    {
        player = GameManager.instance.player.transform;
    }

    private void Update()
    {

        if (player == null) return;

        // OnTriggerExit 대신 Update에서 지속적으로 체크
        if (transform.CompareTag("Ground"))
        {
            CheckAndRepositionTile();
        }
        else if (transform.CompareTag("Enemy"))
        {
            CheckAndRepositionEnemy();
        }
    }

    private void CheckAndRepositionTile()
    {
        if (player == null) return;

        Vector3 playerPos = player.position;
        Vector3 tileMapPos = transform.position;

        float diffX = playerPos.x - tileMapPos.x;
        float diffY = playerPos.y - tileMapPos.y;

        Vector3 newPos = tileMapPos;

        bool needsReposition = false;

        // X축 체크
        if (Mathf.Abs(diffX) > checkDistance)
        {
            newPos.x += Mathf.Sign(diffX) * repositionDistance;
            needsReposition = true;
        }

        // Y축 체크  
        if (Mathf.Abs(diffY) > checkDistance)
        {
            newPos.y += Mathf.Sign(diffY) * repositionDistance;
            needsReposition = true;
        }

        if (needsReposition)
        {
            transform.position = newPos;
            Debug.Log($"Repositioned {gameObject.name} to {newPos}");
        }
    }

    private void CheckAndRepositionEnemy()
    {
        if (!coll.enabled) return;

        Vector3 playerPos = player.position;
        Vector3 enemyPos = transform.position;

        float distance = Vector3.Distance(playerPos, enemyPos);

        if (distance > enemyCheckDistance)
        {
            RepositionEnemy();
        }
    }

    private void RepositionEnemy()
    {
        Vector3 playerPos = player.position;
        Vector3 playerInputVec = GameManager.instance.player.inputVec;

        Vector3 spawnDirection;
        if (playerInputVec != Vector3.zero)
        {
            spawnDirection = playerInputVec.normalized;

            float randomAnle = Random.Range(-45f, 45f);
            spawnDirection = Quaternion.Euler(0, 0, randomAnle) * spawnDirection;
        }
        else
        {
            float randonAngle = Random.Range(0f, 360f);
            spawnDirection = new Vector3(Mathf.Cos(randonAngle * Mathf.Deg2Rad),
                Mathf.Sin(randonAngle * Mathf.Deg2Rad), 0f);
        }

        Vector3 spawnOffset = spawnDirection * enemyCheckDistance;

        spawnOffset += new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);

        Vector3 newPos = playerPos + spawnOffset;
        transform.position = newPos;

        //Debug.Log($"Repositioned Enemy {gameObject.name} to {newPos}");
    }

    // 기존 OnTriggerExit은 백업으로 유지 타일맵용
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 tileMapPos = transform.position;

        float diffX = Mathf.Abs(playerPos.x - tileMapPos.x);
        float diffY = Mathf.Abs(playerPos.y - tileMapPos.y);
        float dirX = Mathf.Sign(playerPos.x - tileMapPos.x);
        float dirY = Mathf.Sign(playerPos.y - tileMapPos.y);

        Vector3 newPos = tileMapPos;

        Vector3 playerDir = GameManager.instance.player.inputVec;

        switch (transform.tag)
        {
            case "Ground":
                if (Mathf.Abs(diffX - diffY) <= 5f)
                {
                    newPos.x += dirX * 40f;
                    newPos.y += dirY * 40f;
                }
                else
                {
                    if (diffX > diffY)
                        newPos.x += dirX * 40f;
                    else
                        newPos.y += dirY * 40f;
                }

                transform.position = newPos;
                break;

            case "Enemy":

                if (coll.enabled)
                {
                    transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
                }

                break;


        }
    }
}*/
#endregion
#region ver1.
/*using UnityEngine;

public class Reposition : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision) //2D트리거 콜라이더에서 벗어날 때 호출되는 유니티 매서드 
    {
        if (!collision.CompareTag("Area")) //만약 아리아 태그가 없으면
            return; //종료

        Vector3 playerPos = GameManager.instance.player.transform.position; //플레이어 현재 위치 게임 매니저에서 받아옴
        Vector3 tileMapPos = transform.position; // 타일맵 위치

        float diffX = Mathf.Abs(playerPos.x - tileMapPos.x);   //플레이어와 오브젝트 간의 x축 거리차
        float diffY = Mathf.Abs(playerPos.y - tileMapPos.y); //플레이어와 오브젝트 간의 Y축 거리차

        float dirX = Mathf.Sign(playerPos.x - tileMapPos.x); //X방향으로의 상대적 위치 방향 계산(Sign = 오른쪽이면 +1, 왼쪽이면 -1)
        float dirY = Mathf.Sign(playerPos.y - tileMapPos.y); //Y방향으로의 상대적 위치 방향  계산 (위쪽이면 +1, 아래쪽이면 -1)

        // Mathf.Sign으로 어느 쪽 방향으로 타일 이동 시켜야 할지 알려주는 부호를 뽑음 오른쪽인지, 왼쪽인지 


        Vector3 newPos = tileMapPos; //현재 오브젝트 위치 기준으로 새로운 위치 선언 해주려고 새 변수 선언 

        //타일맵 태그에 따라 처리 
        switch (transform.tag)
        {
            case "Ground": //만약 그라운드라면 
                if (diffX > diffY) //X축의 차가 Y축의 차보다 크다면
                    newPos.x += dirX * 40f; //수평 이동 해라
                else //아니면 
                    newPos.y += dirY * 40f; //수직 이동 해라 

                transform.position = newPos; //계산한 위치로 오브젝트 직접 이동 
                break;

            case "Enemy":
                // 필요시 구현
                break;
        }
    }
}
*/
#endregion