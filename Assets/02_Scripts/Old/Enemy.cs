/*using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemySpeed; //이동 속도 
    public float enemyHP; //현재 체력
    public float enemyMaxHP; //최고 체력
    public RuntimeAnimatorController[] animCon; //적 종류별 애니메이터 컨트롤러
    public Rigidbody2D target; //추적 대상(플레이어)

    [Header("Drop - Extra")] //드롭아이템(자석, 츄르)
    public int healIndex = 15;                    // 풀 배열 인덱스(힐 프리팹)
    public int magnetIndex = 16;                  // 풀 배열 인덱스(자석 프리팹)
    [Range(0f, 1f)] public float healChance = 0.05f; //츄르 나올 확률
    [Range(0f, 1f)] public float magnetChance = 0.03f; //자석 나올 확률
    public float healValue = 100f;                // 의미 적지만 남겨놓음(어차피 100)
    public float magnetBonusRadius = 10f; //자석 반경
    public float magnetBonusDuration = 5f; //자석 지속성

    [Header("Drop")] //exp 드롭 아이템
    public int silverCoinIndex = 12;   // 은 코인 
    public int goldCoinIndex = 13;   // 금 코인
    public int goldBoxIndex = 14;   // 상자

    [Range(0f, 1f)] public float goldChance = 0.10f; //금코인 나올 확률
    [Range(0f, 1f)] public float goldBoxChance = 0.009f; //상자 나올 확률

    public int dropCountMin = 1; //최소 드롭수
    public int dropCountMax = 2; //최대 드롭수
    public int silverExpValue = 1; //은코인 exp(경험치값)
    public int goldExpValue = 5;//금 코인 경험치 값
    public int goldBoxValue = 20; //박스 경험치 값

    // 넉백 관련 변수 추가
    [Header("Knockback Settings")]
    public float knockbackForce = 3f;      // 넉백 힘
    public float knockbackDuration = 0.2f; // 넉백 지속 시간

    bool isLive; //살아 있나
    bool isKnockBack = false; //넉백?

    WaitForFixedUpdate wait;//코루틴 최적화 
    Rigidbody2D rigid; //리지드바디
    Collider2D coll; //콜라이더
    SpriteRenderer spriter; //스프라이터
    Animator anim; //애니메이터

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        wait = new WaitForFixedUpdate(); //코루틴에서 쓸거라 new 필수

        //컴포넌트 초기화(캐싱?-한 번 찾아서 계속 쓰기)
    }

    void Start()
    {
        RefreshTarget(); //타겟(플레이어 참조)
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive) return;
        if (!isLive) return;

        //게임 실행중 아니거나 죽었으면 돌아가라

        // 타겟이 null인 경우 다시 찾기
        if (target == null)
        {
            RefreshTarget();
            if (target == null) return; // 여전히 null이면 이번 프레임 스킵
        }

        // 넉백 상태 또는 Hit 애니메이션 중이면 추적 중단
        if (isKnockBack || (anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")))
            return;

        //플레이어 찾기
        Vector2 dir = target.position - rigid.position;
        Vector2 nextVect = dir.normalized * enemySpeed * Time.fixedDeltaTime;

        //물리 이동(추적)
        rigid.MovePosition(rigid.position + nextVect);
        //속도 0으로 초기화
        rigid.linearVelocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive || !isLive) return;
        //플레이어 위치에 따라서 고개 돌리기(좌우반전)
        spriter.flipX = target.position.x > rigid.position.x;
    }

    void OnEnable()
    {
        //풀에서 재사용 될 때 초기 세팅
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 3; // ...?레이어였나
        anim.SetBool("Dead", false);
        enemyHP = enemyMaxHP;
        //넉백 상태 초기화
        isKnockBack = false;

        //타겟 재설정 (오브젝트 풀링에서 재사용될 때 안전장치)
        RefreshTarget();

        //애니메이션 상태 초기화
        if (anim != null)
        {
            anim.ResetTrigger("Hit");
           // anim.Play("Walk", 0, 0f); // 기본 상태로 강제 설정 (애니메이션 이름은 실제에 맞게 수정)
        }
    }

    //외부에서 직접 데미지를 주는 함수
    public void OnHit(float damage)
    {
        if (!isLive) return;

        enemyHP -= damage;
        Debug.Log("Enemy HP :" + enemyHP);

        if (enemyHP <= 0)
        {
            //여기서 바로 SetActive(false) 하지 말고 공통 처리로
            Die();  // ← 드롭/사운드/애니까지 한 번에
        }
    }

    //적 생성시 초기 세팅
    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        enemySpeed = data.enemySpeed;
        enemyMaxHP = data.enemyHP;
        enemyHP = data.enemyHP;
    }

    //플레이어 공격에 닿았을 때???
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("CatAttack") || !isLive) return;

        enemyHP -= collision.GetComponent<CatAtack>().damage;

        // [FIX] 넉백은 살아있을 때만 실행
        if (isLive && enemyHP > 0)
        {
            StartCoroutine(KnockBack()); // 넉백
        }

        if (enemyHP > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            Die(); // ← [FIX] 공통 처리 호출
        }
    }

    // =========================
    // [FIX] 넉백 처리 수정 - 더 안전한 버전
    // =========================
    IEnumerator KnockBack()
    {
        // [FIX] 넉백 상태를 true로 설정 (주석 해제)
        isKnockBack = true;

        // [FIX] 한 프레임 대기 후 넉백 힘 적용
        yield return wait;

        // [FIX] 안전 검사 - 아직 살아있고 게임이 진행중인지 확인
        if (!isLive || !GameManager.instance.isLive)
        {
            isKnockBack = false;
            yield break;
        }

        // [FIX] 플레이어로부터 멀어지는 방향으로 넉백
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;

        // [FIX] 넉백 힘 적용 (기존 속도 초기화하지 않음)
        rigid.AddForce(dirVec.normalized * knockbackForce, ForceMode2D.Impulse);

        // [FIX] 넉백 지속 시간만큼 대기
        float elapsed = 0f;
        while (elapsed < knockbackDuration && isLive && GameManager.instance.isLive)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // [FIX] 넉백이 끝나면 속도 초기화하고 상태 해제
        if (rigid != null)
        {
            rigid.linearVelocity = Vector2.zero;
        }
        isKnockBack = false;
    }

    // =========================
    // [NEW] 타겟 참조 새로고침 함수
    // =========================
    void RefreshTarget()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        }
    }

    // =========================
    // [NEW] 공통 사망 처리
    // =========================
    void Die()
    {
        if (!isLive) return;  // 중복 방지
        isLive = false;

        coll.enabled = false;
        rigid.simulated = false;
        spriter.sortingOrder = 1;
        anim.SetBool("Dead", true);
        GameManager.instance.kill++;

        // [FIX] 넉백 상태 해제 및 코루틴 정리
        if (isKnockBack)
        {
            StopCoroutine(KnockBack()); // 실행중인 넉백 코루틴 정지
            isKnockBack = false;
        }

        // [FIX] 속도 초기화
        if (rigid != null)
        {
            rigid.linearVelocity = Vector2.zero;
        }

        if (GameManager.instance.isLive)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);

        DropCoins();                 // ← 드롭 보장
        gameObject.SetActive(false); // 마지막에 비활성화
    }

    // =========================
    // [NEW] 드롭 처리
    // =========================
    void DropCoins()
    {
        int count = Random.Range(dropCountMin, dropCountMax + 1);

        // 코인
        for (int i = 0; i < count; i++)
        {
            bool dropGold = Random.value < goldChance;
            int index = dropGold ? goldCoinIndex : silverCoinIndex;
            int value = dropGold ? goldExpValue : silverExpValue;

            GameObject coin = GameManager.instance.pool.Get(index);
            coin.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.4f;

            var exp = coin.GetComponent<ExpItem>();
            if (exp != null) exp.Init(value);
        }

        // 상자(확률)
        if (Random.value < goldBoxChance)
        {
            GameObject goldBox = GameManager.instance.pool.Get(goldBoxIndex);
            goldBox.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.6f;

            var exp = goldBox.GetComponent<ExpItem>();
            if (exp != null) exp.Init(goldBoxValue);
        }

        if (Random.value < healChance)
        {
            GameObject heal = GameManager.instance.pool.Get(healIndex);
            heal.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.6f;
            heal.GetComponent<DropItem>()?.InitHeal(healValue);
        }
        if (Random.value < magnetChance)
        {
            GameObject mag = GameManager.instance.pool.Get(magnetIndex);
            mag.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.6f;
            mag.GetComponent<DropItem>()?.InitMagnet(magnetBonusRadius, magnetBonusDuration);
        }
    }
}

*//*using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemySpeed;
    public float enemyHP;
    public float enemyMaxHP;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    [Header("Drop - Extra")]
    public int healIndex = 15;                    // 풀 배열 인덱스(힐 프리팹)
    public int magnetIndex = 16;                  // 풀 배열 인덱스(자석 프리팹)
    [Range(0f, 1f)] public float healChance = 0.05f; //츄르 나올 확률
    [Range(0f, 1f)] public float magnetChance = 0.03f; //자석 나올 확률
    public float healValue = 100f;                // 의미 적지만 남겨놓음
    public float magnetBonusRadius = 10f;
    public float magnetBonusDuration = 5f;

    [Header("Drop")]
    public int silverCoinIndex = 12;   // 은 코인 (0부터 시작)
    public int goldCoinIndex = 13;   // 금 코인
    public int goldBoxIndex = 14;   // 상자

    [Range(0f, 1f)] public float goldChance = 0.10f;
    [Range(0f, 1f)] public float goldBoxChance = 0.009f;

    public int dropCountMin = 1;
    public int dropCountMax = 2;
    public int silverExpValue = 1;
    public int goldExpValue = 5;
    public int goldBoxValue = 20;

    // [FIX] 넉백 관련 변수 추가
    [Header("Knockback Settings")]
    public float knockbackForce = 3f;      // 넉백 힘
    public float knockbackDuration = 0.2f; // 넉백 지속 시간

    bool isLive;
    bool isKnockBack = false;

    WaitForFixedUpdate wait;
    Rigidbody2D rigid;
    Collider2D coll;
    SpriteRenderer spriter;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        wait = new WaitForFixedUpdate();
    }

    void Start()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive) return;
        if (!isLive || isKnockBack || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;

        Vector2 dir = target.position - rigid.position;
        Vector2 nextVect = dir.normalized * enemySpeed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nextVect);
        rigid.linearVelocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive || !isLive) return;
        spriter.flipX = target.position.x > rigid.position.x;
    }

    void OnEnable()
    {
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 3;
        anim.SetBool("Dead", false);
        enemyHP = enemyMaxHP;
        // [FIX] 넉백 상태 초기화
        isKnockBack = false;
    }

    public void OnHit(float damage)
    {
        if (!isLive) return;

        enemyHP -= damage;
        Debug.Log("Enemy HP :" + enemyHP);

        if (enemyHP <= 0)
        {
            // [FIX] 여기서 바로 SetActive(false) 하지 말고 공통 처리로
            Die();  // ← 드롭/사운드/애니까지 한 번에
        }
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        enemySpeed = data.enemySpeed;
        enemyMaxHP = data.enemyHP;
        enemyHP = data.enemyHP;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("CatAttack") || !isLive) return;

        enemyHP -= collision.GetComponent<CatAtack>().damage;

        // [FIX] 넉백은 살아있을 때만 실행
        if (isLive && enemyHP > 0)
        {
            StartCoroutine(KnockBack()); // 넉백
        }

        if (enemyHP > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            Die(); // ← [FIX] 공통 처리 호출
        }
    }

    // =========================
    // [FIX] 넉백 처리 수정
    // =========================
    IEnumerator KnockBack()
    {
        // [FIX] 넉백 상태를 true로 설정 (주석 해제)
        isKnockBack = true;

        // [FIX] 한 프레임 대기 후 넉백 힘 적용
        yield return wait;

        // [FIX] 플레이어로부터 멀어지는 방향으로 넉백
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;

        // [FIX] 넉백 힘 적용 (기존 속도 초기화하지 않음)
        rigid.AddForce(dirVec.normalized * knockbackForce, ForceMode2D.Impulse);

        // [FIX] 넉백 지속 시간만큼 대기
        yield return new WaitForSeconds(knockbackDuration);

        // [FIX] 넉백이 끝나면 속도 초기화하고 상태 해제
        rigid.linearVelocity = Vector2.zero;
        isKnockBack = false;
    }

    // =========================
    // [NEW] 공통 사망 처리
    // =========================
    void Die()
    {
        if (!isLive) return;  // 중복 방지
        isLive = false;

        coll.enabled = false;
        rigid.simulated = false;
        spriter.sortingOrder = 1;
        anim.SetBool("Dead", true);
        GameManager.instance.kill++;

        // [FIX] 넉백 상태 해제 (사망 시)
        isKnockBack = false;

        if (GameManager.instance.isLive)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);

        DropCoins();                 // ← 드롭 보장
        gameObject.SetActive(false); // 마지막에 비활성화
    }

    // =========================
    // [NEW] 드롭 처리
    // =========================
    void DropCoins()
    {
        int count = Random.Range(dropCountMin, dropCountMax + 1);

        // 코인
        for (int i = 0; i < count; i++)
        {
            bool dropGold = Random.value < goldChance;
            int index = dropGold ? goldCoinIndex : silverCoinIndex;
            int value = dropGold ? goldExpValue : silverExpValue;

            GameObject coin = GameManager.instance.pool.Get(index);
            coin.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.4f;

            var exp = coin.GetComponent<ExpItem>();
            if (exp != null) exp.Init(value);
        }

        // 상자(확률)
        if (Random.value < goldBoxChance)
        {
            GameObject goldBox = GameManager.instance.pool.Get(goldBoxIndex);
            goldBox.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.6f;

            var exp = goldBox.GetComponent<ExpItem>();
            if (exp != null) exp.Init(goldBoxValue);
        }

        if (Random.value < healChance)
        {
            GameObject heal = GameManager.instance.pool.Get(healIndex);
            heal.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.6f;
            heal.GetComponent<DropItem>()?.InitHeal(healValue);
        }
        if (Random.value < magnetChance)
        {
            GameObject mag = GameManager.instance.pool.Get(magnetIndex);
            mag.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.6f;
            mag.GetComponent<DropItem>()?.InitMagnet(magnetBonusRadius, magnetBonusDuration);
        }
    }
}


*/