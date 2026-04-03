using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayer : MonoBehaviour
{
   
    public Vector2 inputVec;
    public Scanner scanner;

     //public RuntimeAnimatorController[] animCon;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    bool isDead = false;
    private CharacterSO currentCharacterSO;

    [Header("색상")]
    private float hue;
    private float rainbowSpeed = 1f;
    private float goldTime;
    [SerializeField] private GameObject goldParticle;
    [SerializeField] private GameObject rainbowParticle;

    void Awake() //변수 초기화는 awake에서 많이 함
    {
        goldParticle.SetActive(false);
        rainbowParticle.SetActive(false);
        rigid = GetComponent<Rigidbody2D>(); //리지드바디 초기화('캐싱'? - 나중에 자주 쓸 값을 저장해 두는 것, 컴포넌트에 접근 할 수 있게 붙여주기) [초기화, 캐싱 둘 다 맞는 말]
        spriter = GetComponent<SpriteRenderer>(); //스프라이트렌더러 초기화
        anim = GetComponent<Animator>();//애니메이터 초기화
        scanner = GetComponent<Scanner>(); //스캐너 스크립트 초기화

        isDead = false; //죽었나 변수 초기화...꼭 해야하나?-할 필요 없음(아무튼 살아 있는 채로 시작함) , 활성화 비활성화 할 거면 onenable에서 초기화
    }

    void OnEnable() //게임 오브젝트를 활성화 할때마다 호출됨 (onenable에 코루틴 함수를 적용시키고 start에서 초기화 하면 onenable이 더 빨리 호출되기 때문에 실행이 되지 않을 수 있음)
    {
        //speed *= NewChracter.Speed; //주의 onenable때마다 누적 곱해짐(비활성, 활성 되면서 속도가 커질 수 있음)
        currentCharacterSO = GameFacade.Instance.CurrentCharacter;

        if(!currentCharacterSO.isRainbow)
        {
            spriter.color = currentCharacterSO.characterColor;
        }
      

        anim.runtimeAnimatorController = currentCharacterSO.characterController;
        //anim.runtimeAnimatorController = animCon[GameFacade.Instance.PlayerId];
        //선택된 플레이어 id에 해당하는 애니메이터 컨트롤러로 교체
    }


    private void Update()
    {
       RainbowColor(currentCharacterSO);          
       GoldColor(currentCharacterSO); 
    }
    private void FixedUpdate() //물리 연산은 fixedupdate에서
    {
        if (!GameFacade.Instance.IsLive)
            return; //게임 실행중 아니면 돌아가세요

        rigid.AddForce(inputVec); // 힘을 계속 가해서 가속하는 물리적 이동인데, 지금 이동방식이 섞여 있음

        rigid.linearVelocity = inputVec; //속도를 입력벡터로 즉시 지정 -물리 관성 무시

        float speed = GameFacade.Instance.PlayerSpeed;

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime; // MovePosition : 다음 위치를 직접 지정(키네마틱/정확한 이동 느낌, 충돌도 반영)
        rigid.MovePosition(rigid.position + nextVec); //현재 위치+다음 위치 = 움직일 위치
    }
    //쓰레기

    private void LateUpdate() //애니메이션, 스프라이트 후처리는 lateupdate에서 - 왜 적합한지 다시 찾아보기
    {
        if (!GameFacade.Instance.IsLive)
            return; //게임 진행중 아니면 돌아가라

        anim.SetFloat("Speed", inputVec.magnitude); //이동량을 speed 파라미터에 전달(달리기, 걷기 애니메이션) , inputVec.magnitude-벡터 크기 float로 반환하려고, 형변환
        if (inputVec.x != 0) // 좌우 입력에 따라 스프라이트 반전
        {
            spriter.flipX = inputVec.x < 0;// 왼쪽이면 뒤집어라
        }
    }

    private void OnDisable()
    {
        goldParticle.gameObject.SetActive(false);
        rainbowParticle.gameObject.SetActive(false);
    }

    void OnCollisionStay2D(Collision2D collision) //적과 지속 접촉 중일 때 호출
    {
        if (!GameFacade.Instance.IsLive)
            return; //게임 실행 중 아니면 돌아가기

        //GameManager.instance.playerHP -= Time.deltaTime * 20; //적과 닿아 있는 초당 깎이는 체력 oncollisionStay2D는 물리 콜백이라 fixedDeltaTime쓰는 게 이론상 더 정확하다고 함 - 이렇게 쓰면 더 빨리 죽는다고 함

        GameFacade.Instance.PlayerTakeDamage(Time.deltaTime * 20);

        if(GameFacade.Instance.PlayerHp <= 0 && !isDead) //플레이어 체력이 0이하이고 아직 죽지 않았다면
        {
            for (int index = 2; index < transform.childCount; index++) //자식 오브젝트 2번 비활성화(area)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }
            isDead = true; //사망 처리
            anim.SetTrigger("Dead"); //사망 애니메이션

            GameFacade.Instance.GameOver();
            //GameManager.instance.GameOver(); //게임 메니저에서 게임 오버 처리
        
        }
    }
    void OnMove(InputValue value) //input system의 move 액션 콜백(vector2값을 받음)
    {
        inputVec = value.Get<Vector2>();
    }

    private void RainbowColor(CharacterSO character)
    {
        if (!character.isRainbow) return;

        hue += Time.deltaTime * rainbowSpeed;
        hue %= 1f; //0~1까지의 값

        spriter.color = Color.HSVToRGB(hue, 0.45f, 0.95f);     
        rainbowParticle.SetActive(true);

    }

    private void GoldColor(CharacterSO character)
    {
        if(!character.isGold) return;

        goldTime += Time.deltaTime * 10f;
        float brightness = 0.8f + Mathf.Sin(goldTime) * 0.2f;

        spriter.color = character.characterColor * brightness;
        goldParticle.SetActive(true);

    }

}
