/*#region 하악질 냥이

*//*using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
public class Player : MonoBehaviour
{
    public float speed = 1; //�÷��̾� �ӵ� ����
    public Vector2 inputVec; //���� 2 ���� ����
    public Scanner scanner; //��ĳ�� Ŭ���� 
    *//*public CatAcc[] catAccs; //������ �Ǽ��縮-���� ���ý� ����*//*
    public RuntimeAnimatorController[] animCon;
    Rigidbody2D rigid; //������ �ٵ� ������Ʈ 
    SpriteRenderer spriter; //��������Ʈ������ ���Ŵ�
    Animator anim; //�ִϸ����� ���ž�
    bool isDead = false; //�׾���
    bool isAttacking = false; // 공격 중인지 확인하는 변수

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() //start���� ���� ��ũ��Ʈ ��Ȱ��ȭ ���¿����� ȣ�� ������
    {
        rigid = GetComponent<Rigidbody2D>(); //�ʱ�ȭ ������ٵ� ������Ʈ ���ڴ�
        spriter = GetComponent<SpriteRenderer>(); //��������Ʈ ������Ʈ �ʱ�ȭ
        anim = GetComponent<Animator>();//�ִϸ��̼� ������Ʈ �ʱ�ȭ
        scanner = GetComponent<Scanner>(); //��ĳ�� Ŭ���� �ʱ�ȭ 
        *//* catAccs = GetComponentsInChildren<CatAcc>(); //������ ���ݽ� �Ǽ��縮*//*
        isDead = false;
    }
    void OnEnable()
    {
        speed *= Character.Speed; //ĳ���� Ŭ���� Speed
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    // 스페이스바 입력을 처리하는 함수 (Input System 사용)
    void OnAttack(InputValue value)
    {
        if (!GameManager.instance.isLive || isDead)
            return;

        // 공격 애니메이션 실행
        if (!isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    // 공격 실행 코루틴
    IEnumerator PerformAttack()
    {
        isAttacking = true;
        anim.SetTrigger("isEnemy"); // Attack 애니메이션 트리거 실행

        // 트리거 리셋
        yield return null;
        anim.ResetTrigger("isEnemy");

        // 최소 공격 시간 보장 (애니메이션이 보이도록)
        float minAttackTime = 0.2f; // 최소 실행 시간
        float maxAttackTime = 0.8f; // 최대 실행 시간
        float elapsedTime = 0f;

        // 최소 시간까지는 강제로 공격 상태 유지
        while (elapsedTime < minAttackTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최소 시간 이후부터는 움직임 감지 시 종료 가능
        while (elapsedTime < maxAttackTime)
        {
            // 움직임이 감지되면 공격 애니메이션 종료
            if (inputVec.magnitude > 0)
            {
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
    }

    // Update is called once per frame
    *//*    void Update()
        {
            if(!GameManager.instance.isLive) 
            return;
            inputVec.x = Input.GetAxisRaw("Horizontal"); //GetAxisRaw = �� �� �� �������� ������
            inputVec.y = Input.GetAxisRaw("Vertical");
        }*//*

    // 만약 Input System 대신 기존 Input 방식을 사용한다면 Update에서 처리
    void Update()
    {
        if (!GameManager.instance.isLive || isDead)
            return;

        // 스페이스바 입력 확인 (Input System 사용하지 않는 경우)
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private void FixedUpdate() //update�� �Լ� ȣ�� ������ �ٸ��� ��� ȣ�� ������ �����ؼ� �������(������ٵ�), ������Ʈ�� ����
    {
        if (!GameManager.instance.isLive)
            return;
        *//*   rigid.AddForce(inputVec); // ���� ���Ѵ�
           rigid.linearVelocity = inputVec; //�ӵ� ����*//*
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime; //�밢�� �ӵ� ���� �Ϸ��� 
        rigid.MovePosition(rigid.position + nextVec); //���� ��ġ + �Է°� �̷��� ��������
    }
    private void LateUpdate() //�������� ���� �Ǳ� �� ����Ǵ� �����ֱ� �Լ�
    {
        if (!GameManager.instance.isLive)
            return;

        // 공격 중일 때는 Speed를 0으로 설정하여 공격 애니메이션 유지
        if (isAttacking)
        {
            anim.SetFloat("Speed", 0);
        }
        else
        {
            anim.SetFloat("Speed", inputVec.magnitude); //magmitude = ������ ������ ũ�Ⱚ
        }

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }

    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return; //���� ���������� ���� ���� �Ʒ� ������ �ʿ� ����
        GameManager.instance.playerHP -= Time.deltaTime * 20; //적과 닿아 있는 초당 깎이는 체력 
        if (GameManager.instance.playerHP <= 0 && !isDead)
        {
            *//* for (int index = 2; index < transform.childCount; index++)
             {
                 transform.GetChild(index).gameObject.SetActive(false);
             }
 *//*
            isDead = true;
            anim.SetTrigger("Dead"); //���� �� �ִϸ��̼� 
            GameManager.instance.GameOver();
        }
    }
}*//*

#endregion

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public float speed = 1; //플레이어 속도
    public Vector2 inputVec; //입력벡터(좌,우,상,하) - inputsystem 사용
    public Scanner scanner; //스캐너 스크립트 >>private로 바꿔주는 게 좋음<<(awake에서 자식에게 있는 컴포넌트 가져올 수 있어서? 드래그앤드롭 필요 없으니까)

    public RuntimeAnimatorController[] animCon; //애니메이션 컨트롤러 배열


    Rigidbody2D rigid; //리지드바디2d
    SpriteRenderer spriter; //스프라이트 렌더러
    Animator anim; //애니메이터
    bool isDead = false; //죽었나? 확인용 변수
    void Awake() //변수 초기화는 awake에서 많이 함
    {
        rigid = GetComponent<Rigidbody2D>(); //리지드바디 초기화('캐싱'? - 나중에 자주 쓸 값을 저장해 두는 것, 컴포넌트에 접근 할 수 있게 붙여주기) [초기화, 캐싱 둘 다 맞는 말]
        spriter = GetComponent<SpriteRenderer>(); //스프라이트렌더러 초기화
        anim = GetComponent<Animator>();//애니메이터 초기화
        scanner = GetComponent<Scanner>(); //스캐너 스크립트 초기화
        
        isDead = false; //죽었나 변수 초기화...꼭 해야하나?-할 필요 없음(아무튼 살아 있는 채로 시작함) , 활성화 비활성화 할 거면 onenable에서 초기화
    }

    void OnEnable() //게임 오브젝트를 활성화 할때마다 호출됨 (onenable에 코루틴 함수를 적용시키고 start에서 초기화 하면 onenable이 더 빨리 호출되기 때문에 실행이 되지 않을 수 있음)
    {
        speed *= Character.Speed; //주의 onenable때마다 누적 곱해짐(비활성, 활성 되면서 속도가 커질 수 있음)
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
        //선택된 플레이어 id에 해당하는 애니메이터 컨트롤러로 교체
    }

    void OnMove(InputValue value) //input system의 move 액션 콜백(vector2값을 받음)
    {
        inputVec = value.Get<Vector2>();
    }

    // 원래 사용하던 플레이어 이동 방식 
    *//* void Update()
     {
         if (!GameManager.instance.isLive)
             return;

         inputVec.x = Input.GetAxisRaw("Horizontal"); //GetAxisRaw = �� �� �� �������� ������
         inputVec.y = Input.GetAxisRaw("Vertical");
     }*//*

    private void FixedUpdate() //물리 연산은 fixedupdate에서
    {
        if (!GameManager.instance.isLive)
            return; //게임 실행중 아니면 돌아가세요

        rigid.AddForce(inputVec); // 힘을 계속 가해서 가속하는 물리적 이동인데, 지금 이동방식이 섞여 있음

        rigid.linearVelocity = inputVec; //속도를 입력벡터로 즉시 지정 -물리 관성 무시

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime; // MovePosition : 다음 위치를 직접 지정(키네마틱/정확한 이동 느낌, 충돌도 반영)
        rigid.MovePosition(rigid.position + nextVec); //현재 위치+다음 위치 = 움직일 위치
    }
    //쓰레기

    private void LateUpdate() //애니메이션, 스프라이트 후처리는 lateupdate에서 - 왜 적합한지 다시 찾아보기
    {
        if (!GameManager.instance.isLive)
            return; //게임 진행중 아니면 돌아가라

        anim.SetFloat("Speed", inputVec.magnitude); //이동량을 speed 파라미터에 전달(달리기, 걷기 애니메이션) , inputVec.magnitude-벡터 크기 float로 반환하려고, 형변환
        if (inputVec.x != 0) // 좌우 입력에 따라 스프라이트 반전
        {
            spriter.flipX = inputVec.x < 0;// 왼쪽이면 뒤집어라
        }
    }

    void OnCollisionStay2D(Collision2D collision) //적과 지속 접촉 중일 때 호출
    {
        if (!GameManager.instance.isLive) 
            return; //게임 실행 중 아니면 돌아가기

        GameManager.instance.playerHP -= Time.deltaTime * 20; //적과 닿아 있는 초당 깎이는 체력 oncollisionStay2D는 물리 콜백이라 fixedDeltaTime쓰는 게 이론상 더 정확하다고 함 - 이렇게 쓰면 더 빨리 죽는다고 함

        if (GameManager.instance.playerHP <= 0 && !isDead) //플레이어 체력이 0이하이고 아직 죽지 않았다면
        {
            for (int index = 2; index < transform.childCount; index++) //자식 오브젝트 2번 비활성화(area)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }
            isDead = true; //사망 처리
            anim.SetTrigger("Dead"); //사망 애니메이션

            GameManager.instance.GameOver(); //게임 메니저에서 게임 오버 처리
        }
    }

}
*/