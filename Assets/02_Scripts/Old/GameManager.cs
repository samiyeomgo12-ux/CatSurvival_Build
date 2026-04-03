/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{

    public static GameManager instance; //static = 클래스 자체에 속하는 변수, 객체를 만들지 않아도 접근 가능
    //GameManager.instance로 전역 어디에서나 접근할 수 있게 해줌

    [Header("# Game Control")]
    public bool isLive; //현재 게임이 진행 중인가?
    public float gameTime; //현재 진행한 게임 시간
    public float maxGameTime = 5 * 60f; //최대 게임 시간 현재 5분 = 300초
    [Header("# Player Info")] 
    public int playerId; //선택된 플레이어 Id(치즈냥, 흰냥이, 깜냥이, 보라냥)
    public float playerHP; //현재 플레이어 체력
    public float playerMaxHP = 100; //플레이어 최대 체력
    public int level; //플레이어 현재 레벨
    public int kill; //플레이어가 처치한 적의 수
    public int exp; //플레이어 현재 경험치
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 }; //경험치 테이블(해당 경험치를 넘으면 레벨업)
    [Header("# GameObject")]
    public Player player; //플레이어 객체
    public PoolManager pool; // 오브젝트 풀 매니저(에너미, 무기, 아이템 등 관리)
    public LevelUp uiLevelUp; //레벨업 UI
    public Result uiResult; // 결과 UI
    public Transform uiJoy; //모바일용 조이스틱 UI
    public GameObject enemyCleaner; //게임 승리시 적 전부 죽이는 오브젝트

    //[serial....씨리얼그거 쓰자]
    //[HideInInspector] << 이거 쓰면 public으로 써도 인스펙터에서 안보임

    [Header("# Magnet")]
    public float globalMagnetRadius = 0f;  //전역 자석 반경  
    private float magnetEndTime = 0f;  //자석 아이템 효과 종료 시간
                                       

    void Awake() // 오브젝트가 생성되거나(또는 프리펩의 인스턴스화) 초기화 된 직후 호출. 오브젝트를 활성화 시킬 때 단 한 번 함수가 호출되며 스크립트 비활성화 상태라도 호출됨
    {
        instance = this; // 싱글톤 초기화
        Application.targetFrameRate = 60; //안드로이드 어플에서 fps고정(초당 60프레임으로)
    }
    public void GameStart(int id) //어떤 플레이어로 시작하는 지 정보 기억
    {
        playerId = id; //플레이어 id 매개변수(0~3)
        playerHP = playerMaxHP; //플레이어 체력 초기화

        player.gameObject.SetActive(true); //플레이어 오브젝트 활성화
        uiLevelUp.Select(playerId % 2); //플레이어 id에 따라 초기 무기/ 능력 선택
        Resume(); //시간 재개

        AudioManager.instance.PlayBgm(true); //오디오매니저 bgm 활성화
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // 오디오매니저 효과음 재생 
    }

    public void GameOver() //게임 오버처리
    {
        StartCoroutine(GameOverRoutine()); //코루틴 시작
    }

    IEnumerator GameOverRoutine() //코루틴으로 게임 종료 구현
    {
        isLive = false; // 게임 진행 안하고 있음
        yield return new WaitForSeconds(0.5f); // 죽는 애니메이션 보여주려고, 죽음 연출 
        uiResult.gameObject.SetActive(true); //게임 결과Ui 활성화
        uiResult.Lose(); // lose ui 활성화?
        Stop(); //게임 중지

        AudioManager.instance.PlayBgm(false); //오디오매니저에서 bgm 비활성화
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose); //패배 효과음
    }

    public void GameVictory() //게임 승리 처리
    {
        StartCoroutine(GameVictoryRoutine()); //코루틴 시작
    }

    IEnumerator GameVictoryRoutine() //코루틴으로 게임 승리 구현
    {
        isLive = false; //게임 실행 중 아님

        enemyCleaner.SetActive(true); //남아있는 적 제거

        yield return new WaitForSeconds(0.5f); //0.5초 중지 - 애니메이션 

        uiResult.gameObject.SetActive(true); //결과창 활성화
        uiResult.Win(); // 이김 ui 띄우기
        Stop(); //게임 정지

        AudioManager.instance.PlayBgm(false); //배경음 종료
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win); // 승리 효과음
    } 
    public void GameRetry()//개임 재시작
    {
        SceneManager.LoadScene(0); //씬 0번로드 - 내장함수...
    }
    public void GameQuit() //게임 종료
    {
        Application.Quit(); //게임 나가기
    }

    void Update()
    {
        if (!isLive)
            return; //게임이 끝났으면 돌아가라(update 중지), 죽으면 밑에 있는 함수 안쓰려고..?, 이게 없으면 게임타임 게속 + 됨 , 없으면 게임 승리가 계속 실행

        gameTime += Time.deltaTime; //프레임 마다 경과 시간 누적

        if (gameTime > maxGameTime) //최대 게임 타임을 넘으면 승리
        {
            gameTime = maxGameTime;
            GameVictory(); //게임 승리 
        }

    }

    //동전 먹으려 추가하는 코드 
    public void AddExp(int amount) //경험치 추가
    {
        if (!isLive || amount <= 0) return; //게임 진행중이 아니거나, 경험치 수가 0보다 작으면 돌아가시오
        exp += amount; //경험치 증가

        //경험치가 현재 레벨 요구치(경험치 테이블) 이상이면 레벨업 
        while (level < nextExp.Length && exp >= nextExp[level])
        {
            exp -= nextExp[level]; //남은 경험치 버리지 않게 더해주려고?? , 초과된 경험치를 버리지 않고 유지시켜준다
            level++; //레벨 증가
            uiLevelUp.Show(); //레벨업 UI 표시
        }
    }


    public void GetExp() //경험치 1 추가 ...?
    {

        AddExp(1);
       
    } //이거 뭐지...? 실험하려고 쓴 것 같다....쓰레기

    public void Stop() //게임 정지
    {
        isLive = false; //게임 진행 중 아님
        Time.timeScale = 0; // 시간 멈춤
        uiJoy.localScale = Vector3.zero; //조이스틱 숨김
     }
    public void Resume() //게임 재시작
    {
        isLive = true; //게임 진행 중
        Time.timeScale = 1; //게임 시간 다시 흐름
        uiJoy.localScale = Vector3.one * 0.5f; //조이스틱 스케일 좀 줄여서 띄워라
    }
    public void HealFull() // 최대 체력 회복
    {
        if (!isLive) return; //게임 진행중 아니면 돌아가시오
        playerHP = playerMaxHP; //플레이어 체력 최대로

        AudioManager.instance.PlaySfx(AudioManager.Sfx.GetItem); //아이템 먹는 소리
    }
    // =========================
    // 전역 자석 ON (자석 아이템 먹었을 때 호출)
    // =========================
    public void ActivateMagnet(float radius, float duration) //(반경, 지속시간)
    {
        // 기존보다 큰 반경/긴 시간을 먹으면 갱신
        globalMagnetRadius = Mathf.Max(globalMagnetRadius, radius); //  더 큰 반경 반환
        magnetEndTime = Mathf.Max(magnetEndTime, Time.realtimeSinceStartup + Mathf.Max(0.1f, duration)); //자석 아이템 종료 시간(자석 아이템 새로 먹었을 때 더 긴 시간을 선택)
    }

    // =========================
    //전역 자석이 현재 활성화되어 있는지
    // =========================
    public bool IsMagnetActive() //자석 활성화
    {
        if (Time.realtimeSinceStartup > magnetEndTime) // 시간이 끝났다면
        {
            globalMagnetRadius = 0f;                   // 반경 초기화 
            return false;                              // 비활성화
        }
        return globalMagnetRadius > 0f;                // 반경이 0 초과면 활성 상태다
    }
}

//아이템 관리하는 스크립트 하나 있으면 좋을 것 같다-아이템 메니저(싱글톤은 아니고) / 메모리에 아이템 관련 데이터가 상주하고 있는 게 별로 좋지 않다
//그리고 아이템 별로 스크립트를 나누거나 성격이 비슷한 아이템 끼리 묶어서 스크립트 만들기
*/