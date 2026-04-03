using UnityEngine;
using System.Threading.Tasks;


public class GameFlowExample : MonoBehaviour
{
    [SerializeField] private FirebaseBootstrap bootstrap;

    private PlayerService _playerService;
    private RankingService _rankingService;

    private async void Start()
    {
        await bootstrap.InitializeAndSignInAsync();

        _playerService = new PlayerService(FirebaseBootstrap.Db);
        _rankingService = new RankingService(FirebaseBootstrap.Db);

        string uid = FirebaseBootstrap.Uid;

        await _playerService.CreateOrUpdateNicknameAsync(uid, "ев╫╨ем");
        await _playerService.UpdateBestKillsIfHigherAsync(uid, 37);

        var top100 = await _rankingService.LoadTop100Async();

        Debug.Log($"Top100 count : {top100.Count}");
    }
    
}
