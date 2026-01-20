using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoogleMobileAdsDemoScript : MonoBehaviour
{
    // ---------------- Ads ----------------
    private RewardedAd rewardedAd;
    private InterstitialAd interstitialAd;
    private Action pendingRewardCallback;

#if UNITY_ANDROID
    private string rewardedAdUnitId = "ca-app-pub-2920265772112245/3787625121";
    private string interstitialAdUnitId = "ca-app-pub-2920265772112245/9558826552";
#elif UNITY_IPHONE
    private string rewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";
    private string interstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
    private string rewardedAdUnitId = "unused";
    private string interstitialAdUnitId = "unused";
#endif

    public static GoogleMobileAdsDemoScript Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd();
            LoadInterstitialAd();
        });
    }

    // ---------------- Pause / Resume Game ----------------
    public void PauseGame()
    {
       
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        
        Time.timeScale = 1f;
    }

    #region Rewarded Ad
    //public void LoadRewardedAd()
    //{
    //    RewardedAd.Load(rewardedAdUnitId, new AdRequest(),
    //        (RewardedAd ad, LoadAdError error) =>
    //        {
    //            if (error != null || ad == null)
    //            {
    //                Debug.LogError("RewardedAd failed: " + error);
    //                return;
    //            }

    //            Debug.Log("Rewarded ad loaded successfully");
    //            rewardedAd = ad;
    //            rewardedAd.OnAdFullScreenContentClosed += LoadRewardedAd;
    //        });
    //}
    public void LoadRewardedAd()
    {
        RewardedAd.Load(rewardedAdUnitId, new AdRequest(),
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("RewardedAd failed: " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded");
                rewardedAd = ad;

                rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    LoadRewardedAd();
                };

                // 🔥 SHOW immediately if user is waiting
                if (pendingRewardCallback != null && rewardedAd.CanShowAd())
                {
                    Debug.Log("Showing delayed rewarded ad");
                    var callback = pendingRewardCallback;
                    pendingRewardCallback = null;
                    ShowRewardedForStraighten(callback);
                }
            });
    }

    //public void ShowRewardedForStraighten(Action onReward)
    //{
    //    if (rewardedAd != null && rewardedAd.CanShowAd())
    //    {
    //        Debug.Log("Showing rewarded ad");
    //        PauseGame();

    //        rewardedAd.Show(reward =>
    //        {
    //            ResumeGame();
    //            onReward?.Invoke();
    //        });
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Rewarded ad not ready yet, loading...");
    //        LoadRewardedAd();
    //    }
    //}
    public void ShowRewardedForStraighten(Action onReward)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            Debug.Log("Showing rewarded ad");
            PauseGame();

            rewardedAd.Show(reward =>
            {
                ResumeGame();
                onReward?.Invoke();
            });

            return;
        }

        Debug.Log("Rewarded not ready, loading and waiting...");
        pendingRewardCallback = onReward;
        LoadRewardedAd();
    }


    public bool IsRewardedReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }
    #endregion

    #region Interstitial Ad
    public void LoadInterstitialAd()
    {
        InterstitialAd.Load(interstitialAdUnitId, new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Interstitial failed: " + error);
                    return;
                }

                interstitialAd = ad;
                interstitialAd.OnAdFullScreenContentClosed += LoadInterstitialAd;
            });
    }

    public void ShowInterstitialOnRestart(Action afterAd)
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            PauseGame();

            void OnClosedHandler()
            {
                interstitialAd.OnAdFullScreenContentClosed -= OnClosedHandler;
                ResumeGame();
                afterAd?.Invoke();
            }

            interstitialAd.OnAdFullScreenContentClosed += OnClosedHandler;
            interstitialAd.Show();
        }
        else
        {
            afterAd?.Invoke();
        }
    }
    #endregion
}
