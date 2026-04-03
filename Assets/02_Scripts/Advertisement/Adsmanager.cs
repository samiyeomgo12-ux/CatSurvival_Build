using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Runtime.InteropServices;
public class Adsmanager : MonoBehaviour
{
    public static Adsmanager Instance;

    private InterstitialAd interstitial;
    private RewardedAd rewarded;

    [Header("광고 ID")]
    [SerializeField]private string interstitialId = "ca-app-pub-5694177053797182/1845973902";
    [SerializeField]private string rewardedId = "ca-app-pub-5694177053797182/5673538277";
    [SerializeField]private string testInterstitialId = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField]private string testRewardedId = "ca-app-pub-3940256099942544/5224354917";

    public bool IsRewardedReady => rewarded != null && rewarded.CanShowAd();
    public bool IsInterstitialReady => interstitial != null && interstitial.CanShowAd();

    public event Action<int> OnRewardGranted;

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
                return;
            }

            interstitial?.Destroy();
            interstitial = ad;

            interstitial.OnAdFullScreenContentClosed += () =>
            {
                interstitial?.Destroy();
                interstitial = null;
                LoadInterstitial(); // 다시 로드
            };

            interstitial.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogError($"Interstitial show failed :{adError}");
                interstitial?.Destroy();
                interstitial = null;
                LoadInterstitial();
            };
        });
    }

    public void ShowInterstitial()
    {
        if(IsInterstitialReady)
        {
            interstitial.Show();
        }
        else
        {
            Debug.Log("전면광고 준비 안됨");
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
                return;
            }

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
        if (!IsRewardedReady) return;

        rewarded.Show((Reward reward) =>
        {
            Debug.Log("보상 지급");
            GiveReward();
        });
    }

    
    public void GiveReward()
    {
        OnRewardGranted?.Invoke(1); 
    }
}
