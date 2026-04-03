# CatSurvival 리팩토링 설계 문서

> **목적:** 안드로이드 마켓 출시 + 클라이언트 개발자 신입 취업 포트폴리오
> **기간:** 2주 구현 + 1주 마켓 등록
> **엔진:** Unity 6.3

---

## 0. 종합 평가

### ✅ 전체 가능성 평가

| 항목 | 평가 | 이유 |
|------|------|------|
| 안드로이드 출시 | **가능** | 현재 구조도 작동하며, 리팩토링 후 충분히 출시 가능한 수준 |
| 2주 구현 | **빡빡하지만 가능** | 우선순위 조정 필수. 아래 일정표 참고 |
| 취업 어필 | **매우 좋음** | MVP 패턴 + SO 설계 + Firebase + 최적화 조합이 면접에서 강함 |

---

### 💼 클라이언트 개발자 취업 어필 평가

**잘 어필되는 요소:**

| 기술 요소 | 어필 포인트 | 면접 설명 방향 |
|-----------|------------|--------------|
| **MVP 패턴 적용** | 단순 MonoBehaviour 나열을 구조화 | "UI 변경에 게임 로직이 영향받지 않도록 분리했습니다" |
| **ScriptableObject 기반 데이터** | 기획자 친화적 구조 이해 | "데이터를 코드와 분리해 런타임 수정 없이 밸런싱 가능하게 했습니다" |
| **Object Pool 최적화** | 성능 문제 인식 + 해결 경험 | "GC 압박을 줄이기 위해 풀링을 Dictionary 구조로 개선했습니다" |
| **Firebase 연동** | 백엔드 연동 경험 | "실시간 DB로 전체 플레이어 랭킹을 구현했습니다" |
| **모바일 최적화** | 실제 출시 경험 | "DrawCall 배칭, 풀 사이즈 제한, 프레임 타겟 설정 등 적용했습니다" |

**보완하면 더 좋은 것:**
- 스크린샷/영상이 있는 README
- Google Play 실제 출시 링크
- 리팩토링 전/후 성능 비교 (Profiler 캡처)

---

### ⏱️ 2주 일정 평가

**핵심 판단: 6개 항목 전부를 완벽하게 하면 2주 초과. 우선순위 조정 필수.**

```
Week 1 (필수 코어)
├─ Day 1-2: 폴더 구조 재편 + SO 데이터 설계
├─ Day 3-4: Object Pool 리팩토링
├─ Day 5-6: UI MVP 리팩토링
└─ Day 7:   캐릭터 2종 + 특성 아이템 추가

Week 2 (기능 확장)
├─ Day 8-9:   Firebase 랭킹
├─ Day 10:    세이브/로드 시스템 (이어하기)
├─ Day 11:    Unity Ads 광고 연동
├─ Day 12-13: 통합 테스트 + 안드로이드 빌드 최적화
└─ Day 14:    버그 수정 + 마켓 제출 준비
```

> ⚠️ **리스크:** Firebase 연동이 예상보다 오래 걸릴 수 있음 (SDK 설정, 인증 등). 일정 밀리면 광고를 Week 3으로 미루는 것 추천.

---

## 1. 전체 아키텍처 목표 구조

### 현재 구조의 문제점

```
현재 (문제):
GameManager  ──────→  HUD.cs (직접 참조, 강결합)
     │                Result.cs (직접 참조)
     │                LevelUp.cs (직접 참조)
     └──────→  Player.cs (직접 GetComponent, BroadcastMessage)

PoolManager  ──────→  GameObject[] prefebs (인덱스 하드코딩)
                       List<GameObject>[] pools (선형 탐색 O(n))
```

### 목표 구조

```
목표 (개선):
                    ┌─────────────────────────┐
                    │   ScriptableObject Data  │
                    │  (CharacterSO, ItemSO,   │
                    │   EnemySO, GameConfigSO) │
                    └────────────┬────────────┘
                                 │ 데이터만 읽기
          ┌──────────────────────┼──────────────────────┐
          │                      │                      │
    ┌─────▼──────┐        ┌──────▼─────┐        ┌──────▼─────┐
    │ GameManager │        │PoolManager │        │ SaveManager│
    │ (이벤트 발행) │        │(Dictionary)│        │(JSON+Prefs)│
    └─────┬───────┘        └────────────┘        └────────────┘
          │ C# Events / Action
    ┌─────▼────────────────────────────────────────────────┐
    │                    UI Layer (MVP)                     │
    │  ┌──────────────┐  ┌──────────────┐  ┌────────────┐ │
    │  │  HUDPresenter│  │LevelUpPresent│  │ResultPresen│ │
    │  │  (Model→View)│  │  (Model→View)│  │ (Model→View│ │
    │  └──────┬───────┘  └──────┬───────┘  └─────┬──────┘ │
    │         │                 │                 │        │
    │  ┌──────▼───────┐  ┌──────▼───────┐  ┌─────▼──────┐ │
    │  │   HUDView    │  │  LevelUpView │  │ ResultView │ │
    │  └──────────────┘  └──────────────┘  └────────────┘ │
    └──────────────────────────────────────────────────────┘
```

---

## 2. ScriptableObject 데이터 설계

### 2-1. SO 파일 구조

```
Assets/Data/
├── Config/
│   └── GameConfigSO.asset          # 게임 전체 설정 (시간, HP 등)
├── Characters/
│   ├── ChessCatSO.asset
│   ├── WhiteCatSO.asset
│   ├── BlackCatSO.asset
│   ├── PurpleCatSO.asset
│   ├── VampCatSO.asset             # NEW: 체력 흡수 캐릭터
│   └── BombCatSO.asset             # NEW: 폭발 캐릭터
├── Items/
│   ├── Item_CatPaw.asset
│   ├── Item_Glove.asset
│   ├── Item_Shoe.asset
│   ├── Item_VampFang.asset         # NEW: 흡혈 특성 아이템
│   └── Item_BombClaw.asset         # NEW: 폭발 특성 아이템
└── Enemies/
    ├── EnemySpawnConfigSO.asset    # 스폰 웨이브 설정
    └── EnemyDataSO.asset[]         # 적 종류별 데이터
```

### 2-2. 핵심 SO 구조 설계

```csharp
// CharacterSO.cs
[CreateAssetMenu(menuName = "CatSurvival/Character")]
public class CharacterSO : ScriptableObject
{
    [Header("기본 정보")]
    public int id;
    public string characterName;
    public string description;
    public Sprite icon;
    public RuntimeAnimatorController animController;

    [Header("기본 스탯")]
    public float maxHP = 100f;
    public float moveSpeed = 3f;
    public float startDamage = 1f;

    [Header("특성 스탯 보너스 (배율)")]
    public float hpMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float weaponSpeedMultiplier = 1f;
    public float weaponRateMultiplier = 1f;
    public int extraProjectileCount = 0;

    [Header("고유 특성")]
    public CharacterPassiveType passiveType;  // None, VampHeal, BombExplosion
    public float passiveValue;               // 특성 수치 (회복량, 폭발 범위 등)
}

// CharacterPassiveType.cs
public enum CharacterPassiveType
{
    None,
    VampHeal,       // 적 공격 시 체력 회복
    BombExplosion   // 적 처치 시 폭발
}
```

```csharp
// ItemDataSO.cs (기존 ItemData.cs 개선)
[CreateAssetMenu(menuName = "CatSurvival/Item")]
public class ItemDataSO : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoe, Heal, Passive }

    [Header("기본 정보")]
    public int itemId;
    public ItemType itemType;
    public string itemName;
    [TextArea] public string itemDesc;
    public Sprite itemIcon;

    [Header("레벨 업 스탯")]
    public LevelUpStat[] levelStats;  // 레벨별 데이터 배열

    [Header("투사체")]
    public GameObject projectilePrefab;
    public Sprite attackSprite;

    [Header("패시브 특성 (Passive 타입 전용)")]
    public CharacterPassiveType linkedPassive;
    public float passiveValue;
}

[Serializable]
public struct LevelUpStat
{
    public float damage;
    public int count;
    [TextArea] public string description;
}
```

```csharp
// GameConfigSO.cs
[CreateAssetMenu(menuName = "CatSurvival/GameConfig")]
public class GameConfigSO : ScriptableObject
{
    [Header("게임 시간")]
    public float maxGameTime = 300f;

    [Header("플레이어")]
    public float baseMaxHP = 100f;
    public int[] expThresholds;      // 레벨업 경험치 임계값

    [Header("드롭 확률 (0~1)")]
    public float silverCoinChance = 0.9f;
    public float goldCoinChance = 0.1f;
    public float goldBoxChance = 0.009f;
    public float healDropChance = 0.05f;
    public float magnetDropChance = 0.03f;

    [Header("타겟 프레임")]
    public int targetFrameRate = 60;
}
```

---

## 3. Object Pool 리팩토링

### 현재 문제점

```csharp
// 현재: 인덱스 기반, 선형 탐색
public GameObject[] prefebs;
private List<GameObject>[] pools;  // O(n) 탐색, 무한 증가

GameObject obj = PoolManager.instance.Get(7); // 7이 뭔지 모름
```

### 개선 설계

```csharp
// 개선: Dictionary 기반, O(1) 접근, 풀 사이즈 제한
public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public struct PoolConfig
    {
        public string key;          // "Enemy_Bat", "Bullet_Fire" 등
        public GameObject prefab;
        public int initialSize;     // 초기 생성 수
        public int maxSize;         // 최대 풀 사이즈 (GC 방지)
    }

    public PoolConfig[] poolConfigs;

    private Dictionary<string, Queue<GameObject>> pools;
    private Dictionary<string, Transform> poolParents;  // 계층 정리용

    // 사용법
    // PoolManager.instance.Get("Enemy_Bat");
    // PoolManager.instance.Return("Enemy_Bat", obj);
}
```

### 풀 키 네이밍 컨벤션

```
Enemy_[이름]      : Enemy_Bat, Enemy_Skeleton, Enemy_Slime
Bullet_[이름]     : Bullet_Fire, Bullet_Scratch
Coin_[종류]       : Coin_Silver, Coin_Gold, Coin_Box
Item_[종류]       : Item_Heal, Item_Magnet
Effect_[이름]     : Effect_Explosion, Effect_VampHeal
```

### IPoolable 인터페이스

```csharp
public interface IPoolable
{
    void OnSpawn();    // 풀에서 꺼낼 때 초기화
    void OnDespawn();  // 풀에 반납할 때 정리
}

// Enemy.cs에서 구현
public class Enemy : MonoBehaviour, IPoolable
{
    public void OnSpawn()  { /* 스탯 초기화, 콜라이더 활성화 */ }
    public void OnDespawn(){ /* 이펙트 정리, 상태 리셋 */ }
}
```

---

## 4. UI MVP 리팩토링

### MVP 패턴 개요

```
Model  : GameManager의 데이터 (HP, Exp, Level 등) → C# Event로 변경 알림
View   : Unity UI 요소 (Slider, Text, Image) → 표시만 담당
Presenter : Model 이벤트 구독 → View 업데이트
```

### 현재 HUD vs 개선 HUD

```csharp
// 현재 (문제): HUD가 GameManager를 직접 참조
public class HUD : MonoBehaviour
{
    void LateUpdate()
    {
        // 매 프레임 GameManager 폴링
        switch (type)
        {
            case InfoType.HP:
                slider.value = GameManager.instance.playerHP / ...
```

```csharp
// 개선 (MVP): 이벤트 기반, 변경 시에만 업데이트

// --- Model 이벤트 (GameManager.cs에 추가) ---
public static event Action<float, float> OnHPChanged;    // current, max
public static event Action<int, int> OnExpChanged;       // current, max
public static event Action<int> OnLevelChanged;
public static event Action<int> OnKillChanged;
public static event Action<float> OnTimeChanged;

// --- View (HUDView.cs - UI 요소만 보유) ---
public class HUDView : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider expSlider;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text killText;
    [SerializeField] TMP_Text timeText;

    public void SetHP(float current, float max) => hpSlider.value = current / max;
    public void SetExp(float current, float max) => expSlider.value = current / max;
    public void SetLevel(int level) => levelText.text = $"Lv.{level}";
    public void SetKill(int kill) => killText.text = kill.ToString();
    public void SetTime(float time) => timeText.text = $"{(int)(time/60)}:{(int)(time%60):00}";
}

// --- Presenter (HUDPresenter.cs - 연결 담당) ---
public class HUDPresenter : MonoBehaviour
{
    [SerializeField] HUDView view;

    void OnEnable()
    {
        GameManager.OnHPChanged  += view.SetHP;
        GameManager.OnExpChanged += view.SetExp;
        GameManager.OnLevelChanged += view.SetLevel;
        GameManager.OnKillChanged  += view.SetKill;
        GameManager.OnTimeChanged  += view.SetTime;
    }

    void OnDisable()
    {
        GameManager.OnHPChanged  -= view.SetHP;
        GameManager.OnExpChanged -= view.SetExp;
        GameManager.OnLevelChanged -= view.SetLevel;
        GameManager.OnKillChanged  -= view.SetKill;
        GameManager.OnTimeChanged  -= view.SetTime;
    }
}
```

### UI 파일 구조

```
Scripts/UI/
├── HUD/
│   ├── HUDView.cs
│   └── HUDPresenter.cs
├── LevelUp/
│   ├── LevelUpView.cs
│   └── LevelUpPresenter.cs
├── Result/
│   ├── ResultView.cs
│   └── ResultPresenter.cs
├── Ranking/
│   ├── RankingView.cs
│   └── RankingPresenter.cs
└── Common/
    └── UIManager.cs            # 패널 전환 관리
```

---

## 5. 캐릭터 2종 추가 설계

### 5-1. VampCat (흡혈 고양이)

```
특성: 적에게 데미지를 줄 때 데미지의 X%만큼 체력 회복

구현 방식:
CatAttack.cs → 데미지 적용 시 CharacterPassiveType 확인
            → VampHeal이면 GameManager.HealPlayer(damage * healRate) 호출
```

```csharp
// CharacterSO 설정 (VampCatSO.asset)
passiveType  = CharacterPassiveType.VampHeal
passiveValue = 0.1f   // 데미지의 10% 회복
damageMultiplier = 0.9f  // 흡혈 대신 데미지 약간 낮춤 (밸런스)
```

### 5-2. BombCat (폭발 고양이)

```
특성: 적 처치 시 처치된 위치에서 범위 폭발 발생, 주변 적 피해

구현 방식:
Enemy.Die() → CharacterPassiveType 확인
           → BombExplosion이면 PoolManager.Get("Effect_Explosion") 스폰
           → Explosion 오브젝트가 OverlapCircle로 범위 데미지 적용
```

```csharp
// CharacterSO 설정 (BombCatSO.asset)
passiveType  = CharacterPassiveType.BombExplosion
passiveValue = 2.5f   // 폭발 반경
damageMultiplier = 0.8f  // 폭발 대신 직접 데미지 낮춤 (밸런스)
```

### 5-3. 패시브 아이템 연동

```
VampFang 아이템 (Item_VampFang.asset):
  - ItemType = Passive
  - linkedPassive = VampHeal
  - 모든 캐릭터에게 흡혈 특성 부여 (약한 버전)
  - VampCat이 사용하면 수치 강화 (스택)

BombClaw 아이템 (Item_BombClaw.asset):
  - ItemType = Passive
  - linkedPassive = BombExplosion
  - 모든 캐릭터에게 폭발 특성 부여 (약한 버전)
  - BombCat이 사용하면 범위/데미지 강화 (스택)
```

---

## 6. 세이브/로드 시스템 설계

### 저장 범위 결정

```
PlayerPrefs (경량):
  ├─ 캐릭터 해금 정보 (기존 유지)
  ├─ 최고 점수 (킬수, 생존 시간)
  └─ 설정값 (음량, 언어 등)

JSON 파일 (이어하기):
  ├─ 현재 게임 진행 상태
  │   ├─ 플레이어 HP, 레벨, 경험치
  │   ├─ 선택한 무기/기어 목록 + 레벨
  │   ├─ 경과 시간
  │   └─ 캐릭터 ID
  └─ 저장 시점: 레벨업 선택 후 (자연스러운 중단점)
```

### SaveData 구조

```csharp
[Serializable]
public class GameSaveData
{
    public int characterId;
    public float currentHP;
    public float maxHP;
    public int level;
    public int exp;
    public int killCount;
    public float gameTime;
    public List<WeaponSaveData> weapons;
    public List<GearSaveData>   gears;
    public string saveDate;  // ISO 8601
}

[Serializable]
public class WeaponSaveData
{
    public int itemId;
    public int currentLevel;
}

[Serializable]
public class GearSaveData
{
    public int itemId;
    public int currentLevel;
}
```

### SaveManager 핵심 API

```csharp
public class SaveManager : MonoBehaviour
{
    private const string SAVE_FILE = "gamesave.json";

    public void SaveGame(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
        File.WriteAllText(path, json);
    }

    public GameSaveData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<GameSaveData>(File.ReadAllText(path));
    }

    public bool HasSaveData() => File.Exists(
        Path.Combine(Application.persistentDataPath, SAVE_FILE));

    public void DeleteSave() => File.Delete(
        Path.Combine(Application.persistentDataPath, SAVE_FILE));
}
```

### 이어하기 UI 플로우

```
앱 시작
  └─ SaveManager.HasSaveData() == true?
       ├─ YES → "이어하기" 버튼 활성화
       │         └─ 선택 시 LoadGame() → GameManager에 상태 복원
       └─ NO  → 새 게임만 표시

게임 중
  └─ 레벨업 선택 완료 시 → SaveManager.SaveGame() 자동 저장

게임 오버 / 클리어
  └─ SaveManager.DeleteSave() (세이브 초기화)
```

---

## 7. Firebase 랭킹 설계

### 데이터 구조 (Realtime Database)

```json
{
  "rankings": {
    "user_[deviceId]": {
      "nickname": "Player123",
      "score": 1250,
      "killCount": 89,
      "survivalTime": 234.5,
      "characterId": 2,
      "timestamp": 1709500000
    }
  }
}
```

### RankingManager 핵심 구현

```csharp
public class RankingManager : MonoBehaviour
{
    private DatabaseReference dbRef;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dbRef = FirebaseDatabase.DefaultInstance.GetReference("rankings");
        });
    }

    // 점수 제출
    public void SubmitScore(int killCount, float survivalTime)
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        int score = CalculateScore(killCount, survivalTime);

        var entry = new RankingEntry {
            nickname    = PlayerPrefs.GetString("Nickname", "Cat" + Random.Range(1000,9999)),
            score       = score,
            killCount   = killCount,
            survivalTime = survivalTime,
            characterId = GameManager.instance.playerId,
            timestamp   = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        dbRef.Child(deviceId).SetValueAsync(entry.ToDictionary());
    }

    // 상위 100위 조회
    public void FetchTopRankings(Action<List<RankingEntry>> onComplete)
    {
        dbRef.OrderByChild("score")
             .LimitToLast(100)
             .GetValueAsync()
             .ContinueWithOnMainThread(task => {
                 if (task.IsCompleted)
                 {
                     var list = ParseSnapshot(task.Result);
                     list.Sort((a, b) => b.score.CompareTo(a.score));
                     onComplete?.Invoke(list);
                 }
             });
    }

    // 점수 계산 공식
    private int CalculateScore(int kills, float time)
        => kills * 10 + (int)(time * 2);
}
```

### Firebase SDK 설치 순서

```
1. Firebase Console에서 Android 앱 등록
   → Bundle ID: com.yourname.catsurvival
   → google-services.json 다운로드

2. Unity Package Manager
   → Add package from tarball
   → firebase_unity_sdk.zip에서 아래만 설치:
      - FirebaseDatabase.unitypackage
      - FirebaseAuth.unitypackage (익명 인증용)

3. Google Services JSON 위치:
   Assets/google-services.json

4. Project Settings → Player → Bundle Identifier 설정
```

> ⚠️ **주의:** Firebase SDK Unity 패키지는 약 40~80MB입니다.
> 빌드 시간이 증가하므로 IL2CPP + ARM64 타겟으로 설정 권장.

---

## 8. 광고 연동 설계

### 추천: Unity Ads (LevelPlay)

**왜 Unity Ads인가?**

| 광고 SDK | 설치 난이도 | Unity 친화성 | 수익 | 추천 이유 |
|----------|------------|-------------|------|----------|
| **Unity Ads** | ⭐ 매우 쉬움 | ⭐⭐⭐ 최고 | 보통 | Package Manager 1클릭 설치 |
| Google AdMob | 중간 | 보통 | 높음 | 설정 복잡, AAR 충돌 위험 |
| AppLovin MAX | 어려움 | 보통 | 높음 | 사용 제외 조건 해당 |

### 설치 방법

```
Window → Package Manager → Unity Registry 검색
→ "Advertisement Legacy" 또는 "Unity Ads SDK" 설치
→ Project Settings → Services → Ads 활성화
```

### 광고 노출 타이밍 설계

```
게임 오버 화면:
  └─ "결과 확인" 클릭 → 전면 광고(Interstitial) 표시

레벨업 선택 전:
  └─ 5레벨마다 광고 1회 (너무 자주 X)

부활 기능 (선택적 구현):
  └─ 보상형 광고(Rewarded) 시청 → 30% HP로 부활 1회
```

### AdsManager 핵심 구조

```csharp
public class AdsManager : MonoBehaviour
{
    const string GAME_ID_ANDROID = "YOUR_GAME_ID";
    const string INTERSTITIAL_ID = "Interstitial_Android";
    const string REWARDED_ID     = "Rewarded_Android";

    void Start()
    {
        Advertisement.Initialize(GAME_ID_ANDROID, testMode: false);
        LoadInterstitial();
        LoadRewarded();
    }

    public void ShowInterstitial(Action onComplete = null) { ... }
    public void ShowRewarded(Action onRewarded, Action onSkipped = null) { ... }

    // 광고 로드 실패해도 게임 진행에 문제없도록 방어 코드 필수
}
```

---

## 9. 프로젝트 폴더 구조 (목표)

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs          # 게임 상태 + 이벤트 발행
│   │   ├── PoolManager.cs          # Dictionary 기반 풀
│   │   ├── AudioManager.cs
│   │   ├── SaveManager.cs          # JSON 세이브
│   │   └── AdsManager.cs           # Unity Ads
│   ├── Character/
│   │   ├── Player.cs
│   │   ├── CharacterStats.cs       # 기존 Character.cs 개선
│   │   └── PassiveHandler.cs       # 흡혈/폭발 패시브 처리
│   ├── Enemy/
│   │   ├── Enemy.cs
│   │   ├── Spawner.cs
│   │   ├── Scanner.cs
│   │   └── Reposition.cs
│   ├── Weapon/
│   │   ├── WeaponManager.cs
│   │   ├── CatAttack.cs
│   │   ├── CatScratch.cs
│   │   └── Gear.cs
│   ├── Item/
│   │   ├── ExpItem.cs
│   │   ├── DropItem.cs
│   │   └── Magnet.cs
│   ├── UI/
│   │   ├── HUD/
│   │   │   ├── HUDView.cs
│   │   │   └── HUDPresenter.cs
│   │   ├── LevelUp/
│   │   │   ├── LevelUpView.cs
│   │   │   └── LevelUpPresenter.cs
│   │   ├── Result/
│   │   │   ├── ResultView.cs
│   │   │   └── ResultPresenter.cs
│   │   ├── Ranking/
│   │   │   ├── RankingView.cs
│   │   │   └── RankingPresenter.cs
│   │   └── Common/
│   │       ├── UIManager.cs
│   │       └── ItemSlot.cs         # 기존 Item.cs 역할
│   ├── Firebase/
│   │   └── RankingManager.cs
│   └── Data/
│       ├── CharacterSO.cs
│       ├── ItemDataSO.cs
│       ├── EnemyDataSO.cs
│       └── GameConfigSO.cs
├── Data/                            # .asset 파일들
│   ├── Config/
│   ├── Characters/
│   ├── Items/
│   └── Enemies/
├── Prefabs/                         # 기존 prefebs 이름 수정
│   ├── Enemies/
│   ├── Weapons/
│   ├── Items/
│   └── Effects/
└── Art/
    ├── Characters/
    ├── Enemies/
    ├── UI/
    └── Tiles/
```

---

## 10. 모바일 최적화 체크리스트

### Android 빌드 설정

```
Project Settings → Player:
  ✅ Scripting Backend: IL2CPP
  ✅ Target Architectures: ARM64 (ARMv7 제외 가능)
  ✅ Managed Stripping Level: Medium
  ✅ Bundle Identifier: com.yourname.catsurvival

Project Settings → Quality:
  ✅ Target Frame Rate: 60 (GameConfigSO에서 Application.targetFrameRate = 60)
  ✅ VSync: Don't Sync (모바일은 VSync 끔)

Project Settings → Graphics:
  ✅ Renderer: Universal Render Pipeline (URP) 확인
```

### 런타임 최적화 포인트

```
1. Pool 사이즈 제한
   - 각 풀당 maxSize 설정 → 메모리 상한선 유지
   - Enemy 풀: 최대 30개, Bullet 풀: 최대 50개

2. Update() 최소화
   - HUD: 이벤트 기반으로 교체 (매 프레임 폴링 제거)
   - Magnet: Update() → 코루틴으로 변경 (0.1초 간격)
   - Scanner: FixedUpdate로 이동

3. 스프라이트 아틀라스
   - UI 스프라이트 → 하나의 Atlas로 묶기
   - DrawCall 감소

4. 카메라
   - Camera.main 캐싱 (매 호출마다 Find 방지)
   - 불필요한 후처리 효과 제거
```

---

## 11. 구현 우선순위 요약

### 반드시 해야 함 (취업 + 출시 필수)
1. ✅ Object Pool 리팩토링 (Dictionary 구조)
2. ✅ UI MVP 리팩토링 (HUD + Result 최소)
3. ✅ SO 데이터 구조 정리 (기존 ItemData → ItemDataSO)
4. ✅ 캐릭터 2종 추가 (CharacterSO 기반)
5. ✅ 안드로이드 빌드 최적화

### 하면 많이 좋아짐 (취업 어필 강화)
6. ✅ Firebase 랭킹
7. ✅ 세이브/로드 (이어하기)

### 시간 되면 추가 (없어도 출시 가능)
8. ⬜ Unity Ads 광고
9. ⬜ 부활 광고 (Rewarded)

---

## 12. 면접 대비 Q&A 예상 질문

**Q: 왜 MVP 패턴을 선택했나요?**
> "Unity의 MonoBehaviour는 MVC의 Controller 역할이 명확하지 않아 UI와 게임 로직이 섞이기 쉬웠습니다.
> MVP에서 Presenter가 View와 GameManager 사이를 중재하도록 설계해 UI 변경 시 게임 로직에 영향이 없도록 했습니다."

**Q: 오브젝트 풀을 왜 List에서 Dictionary로 바꿨나요?**
> "기존 List 기반은 비활성 오브젝트를 찾을 때 O(n) 선형 탐색을 했습니다.
> Dictionary<string, Queue<GameObject>> 구조로 바꾸면 O(1)에 접근 가능하고,
> string 키로 명시적으로 오브젝트 타입을 알 수 있어 유지보수도 편해졌습니다."

**Q: ScriptableObject를 왜 사용했나요?**
> "인스펙터에서 기획 데이터를 수정할 수 있고, 프리팹이나 씬에 종속되지 않아 재사용성이 높습니다.
> 또한 런타임에 수정해도 AssetDatabase에 반영되지 않아 테스트와 실 데이터를 분리할 수 있었습니다."

**Q: Firebase 랭킹에서 치팅 방지는 어떻게 했나요?**
> "클라이언트 제출 데이터를 그대로 신뢰하지 않고, 점수 공식(킬수 × 10 + 생존시간 × 2)을
> 서버 사이드 Rules로 검증하거나 허용 범위(최대 생존시간 300초 등)를 Firebase Rules로 제한했습니다."

---

*마지막 업데이트: 2026-03-04*
*작성: Claude (CatSurvival 리팩토링 설계 지원)*
