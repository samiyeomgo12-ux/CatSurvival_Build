using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Runtime.InteropServices;
public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    private InterstitialAd interstitial;
    private RewardedAd rewarded;

    [Header("광고 ID")]
    [SerializeField]private string interstitialId = "ca-app-pub-5694177053797182/1845973902";
    [SerializeField]private string rewardedId = "ca-app-pub-5694177053797182/5673538277";
    [SerializeField]private string testInterstitialId = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField]private string testRewardedId = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField] private float reloadInitialDelay = 2f;
    [SerializeField] private float reloadMaxDelay = 60f;
    private float rewardedReloadDelay;
    private float interstitialReloadDelay;
    public bool IsRewardedReady => rewarded != null && rewarded.CanShowAd();
    public bool IsInterstitialReady => interstitial != null && interstitial.CanShowAd();
    private Action pendingInterstitialClosedCallback;
    public event Action<int> OnRewardGranted;
    public event Action OnRewardedNotReady;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
            
        Instance = this;

        DontDestroyOnLoad(gameObject);
         
    }
    private void Start()
    {
        MobileAds.Initialize(_=>
        {
            LoadInterstitial();
            LoadRewarded();
        });  


    }

    
    private void LoadInterstitial()
    {
        AdRequest request = new AdRequest();

        InterstitialAd.Load(
        testInterstitialId,
        request,
        (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.Log("Interstitial load failed");
                InterstitialReload();
                return;
            }
            interstitialReloadDelay = 0;
            interstitial?.Destroy();
            interstitial = ad;

            interstitial.OnAdFullScreenContentClosed += () =>
            {
                pendingInterstitialClosedCallback?.Invoke();
                pendingInterstitialClosedCallback = null;
                interstitial?.Destroy();
                interstitial = null;
                LoadInterstitial(); // 다시 로드
            };

            interstitial.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogError($"Interstitial show failed :{adError}");
                pendingInterstitialClosedCallback?.Invoke();
                pendingInterstitialClosedCallback = null;
                interstitial?.Destroy();
                interstitial = null;
                LoadInterstitial();
            };
        });
    }

    public void ShowInterstitial(Action onClosedOnFaild = null)
    {
        if(IsInterstitialReady)
        {
            pendingInterstitialClosedCallback = onClosedOnFaild;
            interstitial.Show();
        }
        else
        {
            Debug.Log("전면광고 준비 안됨");
            onClosedOnFaild?.Invoke();
        }
    }
    private void LoadRewarded()
    {
        AdRequest request = new AdRequest();

        RewardedAd.Load(
            testRewardedId, 
            request, 
            (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"Rewarded load failed: {error}");
                RewardedReload();
                return;
            }

            rewardedReloadDelay = 0;
            rewarded?.Destroy();
            rewarded = ad;

            rewarded.OnAdFullScreenContentClosed += () =>
            {
                rewarded?.Destroy();
                rewarded = null;
                LoadRewarded();
            };

            rewarded.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogWarning($"Rewarded show failed: {adError}");
                rewarded?.Destroy();
                rewarded = null;
                LoadRewarded();
            };
        });
    }

  
    public void ShowRewarded()
    {
        if (!IsRewardedReady)
        {
            OnRewardedNotReady?.Invoke(); //광고 없음 사실 알림
            return;
        }

        rewarded.Show((Reward reward) =>
        {
            Debug.Log("보상 지급");
            GiveReward();
        });
    }

    private void InterstitialReload()
    {
        interstitialReloadDelay = interstitialReloadDelay <= 0 ?
            reloadInitialDelay : Mathf.Min(interstitialReloadDelay * 2f, reloadMaxDelay);

        StartCoroutine(ReloadAfter(interstitialReloadDelay, LoadInterstitial));
    }
    private void RewardedReload()
    {
        rewardedReloadDelay = rewardedReloadDelay <= 0 ?
            reloadInitialDelay : Mathf.Min(rewardedReloadDelay * 2f, reloadMaxDelay);

        StartCoroutine(ReloadAfter(rewardedReloadDelay, LoadRewarded));
    }

    private IEnumerator ReloadAfter(float delay, Action loader)
    {
        yield return new WaitForSecondsRealtime(delay);
        loader();
    }

    public void GiveReward()
    {
        OnRewardGranted?.Invoke(1); 
    }
}
