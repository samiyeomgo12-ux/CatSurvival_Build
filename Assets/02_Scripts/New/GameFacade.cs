

using UnityEngine;
using System;
using System.Threading.Tasks;

public class GameFacade : MonoBehaviour
{
    public static GameFacade Instance;

    [SerializeField] private NewObjectPoolManager poolManager;
    [SerializeField] private NewGameManager gameManager;
    [SerializeField] private NewPlayerManager playerManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private NewUIManager uiManager;
    [SerializeField] private NewAudioManager audioManager;
    [SerializeField] private NewItemManager itemManager;
    [SerializeField] private LifeManager lifeManager;

    public bool IsLive => gameManager.isLive;
    public float GameTime => timeManager.GameTime;
    public float[] MaxGameTime => timeManager.MaxGameTime;
    public int CurrentMaxGameTimeIdx => timeManager.currentMaxGameTimeIdx;
    public NewPlayer Player => playerManager.Player;    
    public float PlayerHp => playerManager.PlayerHp;
    public float PlayerSpeed => playerManager.PlayerSpeed;
    public int PlayerId => playerManager.PlayerId;
    public int Exp => playerManager.Exp;
    public int Kill => playerManager.Kill;
    public NewPlayerManager NewPM => playerManager;
    public LifeManager LifeManager => lifeManager;
    public CharacterSO CurrentCharacter => playerManager.CharacterData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        gameManager.OnStop += () => uiManager.uiJoy.localScale = Vector3.zero;
        gameManager.OnResume += () => uiManager.uiJoy.localScale = Vector3.one * 0.5f;
        playerManager.OnLevelup += Levelup;
        gameManager.OnGameVictiory += uiManager.Win;
        gameManager.OnGameVictiory += () => _ = UpLoadBestKillsOnGameEndAsync();
        gameManager.OnGameOver +=  uiManager.Lose;
        gameManager.OnGameOver += uiManager.PauseAndResumeButtonHide;
        gameManager.OnGameOver += () => _ = UpLoadBestKillsOnGameEndAsync();

        gameManager.OnStop += () => timeManager.SetLive(false);
        gameManager.OnResume += () => timeManager.SetLive(true);
        timeManager.OnTimeUp += gameManager.GameVictory;


    }

    private void OnDestroy()
    {
        gameManager.OnStop -= () => uiManager.uiJoy.localScale = Vector3.zero;
        gameManager.OnResume -= () => uiManager.uiJoy.localScale = Vector3.one * 0.5f;

        gameManager.OnGameVictiory -= () => uiManager.Win();
        gameManager.OnGameOver -= () => uiManager.Lose();
    }
    public void JoyShowAndHide() => uiManager.JoyShowAndHide();
    public void Levelup()
    {
        uiManager.uiLevelUp.Show(); 
    }
    public void GameStart(CharacterSO characterData)
    {
        playerManager.Init(characterData);     
        //uiManager.uiLevelUp.Select(id % 2);
        itemManager.InitWeapon(characterData.baseWeaponKey);
        uiManager.PauseAndResumeButtonShow();
        gameManager.GameResume();
        playerManager.PlayerActive();
        audioManager.PlayBgm(true);
        audioManager.EffectBgm(false);
    }
    public void PlayerExpAdd(int amount)
    {
        playerManager.AddExp(amount);
    }
    public void PlayerSpeedUp(float rate)
    {
        playerManager.SpeedUp(rate);
    }
    public void PlayerHealFull()
    {
        playerManager.HealFull();
    }
    public void PlayerTakeDamage(float amount)
    {
        playerManager.TakeDamage(amount);
    }
    public void GameResume()
    {
        gameManager.GameResume();
        audioManager.PlayBgm(true);      
    }

    private async Task UpLoadBestKillsOnGameEndAsync()
    {
        try
        {
            string uid = FirebaseBootstrap.Uid;

            if(string.IsNullOrEmpty(uid))
            {
                Debug.Log("Uid 없음 : bestKillcount업로드 불가");
                return;
            }

            int currentKills = Kill;

            var playerService = new PlayerService(FirebaseBootstrap.Db);
            await playerService.UpdateBestKillsIfHigherAsync(uid, currentKills);

            Debug.Log($"bestKills 업로드 시도 완료 {currentKills}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"bestKills 업로드 실패 :{ex}");
        }
    }
    public void TryRetryWithLifeCost()
    {
        if(!lifeManager.CanPlay())
        {
            Debug.Log("하트 부족 플레이 불가");
            return;
        }

        bool consumed = lifeManager.ConsumeLife();

        if (!consumed)
        {
            Debug.Log("하트 차감 실패");
            return;
        }
        if(Adsmanager.Instance != null)
        {
            Adsmanager.Instance.ShowInterstitial(() =>
            {
                gameManager.GameRetry();
                audioManager.PlayBgm(false);
            });
            return;
        }
        gameManager.GameRetry();
        audioManager.PlayBgm(false);
        return;

    }
    public void GameRetry()
    {
        if(Adsmanager.Instance != null)
        {
            Adsmanager.Instance.ShowInterstitial(() =>
            {
                gameManager.GameRetry();
                audioManager.PlayBgm(false);
            });
            return;
        }
        gameManager.GameRetry();
        audioManager.PlayBgm(false);        
    }
    public void GameExit() => gameManager.GameQuit();
    public void GameOver()
    {
        gameManager.GameOver();
        audioManager.PlayBgm(false);
    }
    public void GameStop()
    {
        gameManager.Stop();
        audioManager.PlayBgm(false);
    }    
    public void AddKill()
    {
        playerManager.AddKill();
    }
    public void SetBGMVolume(float value)
    {
        audioManager.SetBGMVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        audioManager.SetSFXVolume(value);
    }
    public void PlaySfx(Sfx sfx)
    {
        audioManager.PlaySfx(sfx);
    }
    public void EffectBgm(bool isPlay)
    {
        audioManager.EffectBgm(isPlay);
    }
    public void ActivateMagnet(float radius, float duration)
        => playerManager.ActivateMagnet(radius, duration);
    public bool IsMagnetActive() => playerManager.IsMagnetActive();
    public float MagnetRadius => playerManager.MagnetRadius;

    //아이템 
    public void SelectItem(int idx) => itemManager.SelectItem(idx);
    public ItemData GetItemData(int idx) => itemManager.GetData(idx);
    public int GetItemLevel(int idx) => itemManager.GetLevel(idx);
    public int GetItemCount(int idx) => itemManager.ItemCount;
    public bool IsItemMaxLevel(int idx) => itemManager.IsMaxLevel(idx);


    //오브젝트 풀 
    public GameObject Get<T>(T key) where T : System.Enum
    {
        return key switch
        {
            EnemyEnumId enemyKey => poolManager.Get(enemyKey),
            WeaponEnumId weaponKey => poolManager.Get(weaponKey),
            ItemEnumId itemkey => poolManager.Get(itemkey),
            _ => null
        };
    }
    public void Return<T>(T key, GameObject obj) where T : System.Enum
    {
        switch (key)
        {
            case EnemyEnumId enemyKey: poolManager.Return(enemyKey, obj); break;
            case WeaponEnumId weaponKey: poolManager.Return(weaponKey, obj); break;
            case ItemEnumId itemKey: poolManager.Return(itemKey, obj); break;
        }
    }

}
