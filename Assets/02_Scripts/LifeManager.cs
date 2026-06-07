using System;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private LifeUIView lifeUIView;

    private const string LifeKey = "LIFE_CURRENT";
    private const string LastTickKey = "LIFE_LAST_TICK";

    [Header("라이프 세팅")]
    [SerializeField] private int maxLife = 5;
    [SerializeField] private int rechargeminutes = 5;

    public int CurrentLife { get; private set; }
    public int MaxLife => maxLife;

    public event Action<int> OnLifeChanged;
    public event Action<TimeSpan> OnTimerChanged;

    private DateTime _nextRechargeTime;

    private void Awake()
    {
        Load();
        Recoverlife();
        NotifyAll();
    }
    private void Start()
    {
        TrySubscribeAdsReward();
    }
    private void Update()
    {
        Recoverlife();

        if(CurrentLife < maxLife)
        {
            TimeSpan remain = _nextRechargeTime - DateTime.UtcNow; //다음충전 시간 - 현재시간 (남은 시간)

            if(remain <TimeSpan.Zero)
                remain = TimeSpan.Zero;

            OnTimerChanged?.Invoke(remain);
        }

    }

    private void OnEnable()
    {
        if (AdsManager.Instance != null)
            AdsManager.Instance.OnRewardGranted += HandleReward;
    }

    private void OnDisable()
    {
        if(AdsManager.Instance != null)
            AdsManager.Instance.OnRewardGranted -= HandleReward;
    }


    private void TrySubscribeAdsReward()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.OnRewardGranted -= HandleReward;
            AdsManager.Instance.OnRewardGranted += HandleReward;
        }
    }
    private void HandleReward(int amount)
    {
        AddLife(amount);
    }
    public bool CanPlay()
    {
        Recoverlife();
        return CurrentLife > 0;

    }

    public bool ConsumeLife()
    {
        Recoverlife();

        if (CurrentLife <= 0)
            return false;

        CurrentLife--;

        if(CurrentLife < maxLife)
        {
            SaveTime(DateTime.UtcNow);//하트 소비 시간 기준 충전 시작 
            _nextRechargeTime = DateTime.UtcNow.AddMinutes(rechargeminutes);
        }

        Save();
        NotifyAll();
        return true;
    }

    public void AddLife(int amount)
    {
        Recoverlife();

        CurrentLife = Mathf.Clamp(CurrentLife + amount, 0, maxLife);

        if(CurrentLife >= maxLife)
        {
            SaveTime(DateTime.UtcNow);
        }

        Save();
        NotifyAll();
    }

    public TimeSpan GetRemainingTime()
    {
        Recoverlife();

        if(CurrentLife >= maxLife)
            return TimeSpan.Zero;

        TimeSpan remain = _nextRechargeTime - DateTime.UtcNow;
        return remain < TimeSpan.Zero ? TimeSpan.Zero : remain;
    }

    private void Recoverlife()
    {
        if (CurrentLife >= maxLife)
        {
            _nextRechargeTime = DateTime.UtcNow;
            return;
        }

        DateTime lastTick = LoadTime(); //마지막으로 충전 기준이 된 시간 
        DateTime now = DateTime.UtcNow; //현재시간 
        TimeSpan rechargeSpan = TimeSpan.FromMinutes(rechargeminutes);

        if(now <=  lastTick)//시간 역행 방어(현재시간이 마지막 충전 시간보다 같거나 작을 떄)
        {
            _nextRechargeTime = lastTick.Add(rechargeSpan);
            return;
        }

        TimeSpan elapsed = now - lastTick; //마지막 기준 시각부터 얼마자 시간이 지났나 

        int recovered = Mathf.FloorToInt((float)(elapsed.TotalSeconds / rechargeSpan.TotalSeconds)); // 경과 시간에 하트 몇개 충전?

        if(recovered > 0)
        {
            CurrentLife = Mathf.Clamp(CurrentLife + recovered, 0, maxLife);

            if(CurrentLife >= maxLife)
            {
                lastTick = now; //풀충전시 기준 시각을 현재 시각으로 초기화
            }
            else
            {
                lastTick = lastTick.AddSeconds(recovered * rechargeSpan.TotalSeconds);
            }

            //갱신 기준 시간 저장
            SaveTime(lastTick);
            SaveLife(CurrentLife);
        }

        _nextRechargeTime = lastTick.Add(rechargeSpan); //다음 하트 충전될 시간 
    }

    private void Load()
    {
        CurrentLife = PlayerPrefs.GetInt(LifeKey, maxLife);

        if (!PlayerPrefs.HasKey(LastTickKey))
        {
            SaveTime(DateTime.UtcNow); //시간 정보가 없으면 현재 시간으로 불러오기 
        }
        _nextRechargeTime = LoadTime().AddMinutes(rechargeminutes);
    }

    private void Save()
    {
        SaveLife(CurrentLife);
        if(CurrentLife >= maxLife)
        {
            SaveTime(DateTime.UtcNow);
        }
        PlayerPrefs.Save();
    }

    private void SaveLife(int life)
    {
        PlayerPrefs.SetInt(LifeKey, life);
    }

    private void SaveTime(DateTime time)
    {
        //DateTime을 이진수 형태로 변환해서 저장(문자열이지만, long)
        PlayerPrefs.SetString(LastTickKey, time.ToBinary().ToString());
    }

    private DateTime LoadTime()
    {
        string raw = PlayerPrefs.GetString(LastTickKey,
            DateTime.UtcNow.ToBinary().ToString());

        if(long.TryParse(raw, out long binary))
            return DateTime.FromBinary(binary);

        return DateTime.UtcNow;
    }

    private void NotifyAll()
    {
        OnLifeChanged?.Invoke(CurrentLife);
        OnTimerChanged?.Invoke(GetRemainingTime());
    }
}
