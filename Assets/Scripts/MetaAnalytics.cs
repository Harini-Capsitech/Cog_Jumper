using Facebook.Unity;
using UnityEngine;
using System.Collections.Generic;

public class MetaAnalytics : MonoBehaviour
{
    public static MetaAnalytics Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(OnInitComplete);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    void OnInitComplete()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            Debug.Log("Meta Analytics Initialized");
        }
    }

    public void LogGameStart()
    {
        if (!FB.IsInitialized) return;
        FB.LogAppEvent("game_start");
    }

    public void LogGameOver(int score)
    {
        if (!FB.IsInitialized) return;

        FB.LogAppEvent(
            "game_over",
            parameters: new Dictionary<string, object>
            {
                { "score", score }
            }
        );
    }
}
