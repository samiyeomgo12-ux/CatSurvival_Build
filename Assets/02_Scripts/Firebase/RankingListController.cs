using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;

public class RankingListController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private FirebaseBootstrap bootstrap;   // 이전에 만든 Bootstrap
    private RankingService _rankingService;                 // 이전에 만든 RankingService

    [Header("UI")]
    [SerializeField] private GameObject rankingObjRoot;
    [SerializeField] private Transform contentRoot;         // ScrollView/Viewport/Content
    [SerializeField] private RankingView rankingPrefab;    // 랭킹 행 프리팹
    [SerializeField] private Text stateText;            // 로딩/에러 표시용(선택)
    [SerializeField] private Button closeButton;

    [Header("Options")]
    [SerializeField] private int maxCount = 100;

    private readonly List<RankingView> _spawned = new();

    private void Start()
    {
        closeButton.onClick.AddListener(() => rankingObjRoot.SetActive(false));
    }
    private async void OnEnable()
    {
        await EnsureReadyAndLoadAsync();
    }

    public void Close()
    {
        rankingObjRoot.SetActive(false);
    }

    public async Task EnsureReadyAndLoadAsync()
    {
        SetState("로딩 중...");

        // Firebase 초기화/로그인 보장
        await bootstrap.InitializeAndSignInAsync();

        _rankingService = new RankingService(FirebaseBootstrap.Db);

        try
        {
            await LoadAndRenderAsync();
            SetState(""); // 상태문구 숨김
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ranking load failed: {e}");
            SetState("랭킹을 불러오지 못했습니다.\n잠시 후 다시 시도해주세요.");
        }
    }

    public async Task LoadAndRenderAsync()
    {
        List<PlayerData> data = await _rankingService.LoadTop100Async();

        ClearItems();

        int count = Mathf.Min(maxCount, data.Count);
        for (int i = 0; i < count; i++)
        {
            var d = data[i];
            var item = Instantiate(rankingPrefab, contentRoot);
            item.Bind(
                rank: i + 1,
                nickname: string.IsNullOrEmpty(d.nickname) ? d.uid : d.nickname,
                uid: d.uid,
                bestKills: d.bestKills
            );
            _spawned.Add(item);
        }

        if (count == 0)
        {
            SetState("아직 랭킹 데이터가 없습니다.");
        }
    }

    public async void OnClickRefresh()
    {
        await EnsureReadyAndLoadAsync();
    }

    private void ClearItems()
    {
        for (int i = 0; i < _spawned.Count; i++)
        {
            if (_spawned[i] != null) Destroy(_spawned[i].gameObject);
        }
        _spawned.Clear();
    }

    private void SetState(string msg)
    {
        if (stateText != null) stateText.text = msg;
    }
}