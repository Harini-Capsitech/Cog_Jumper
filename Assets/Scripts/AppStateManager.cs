#nullable disable

using System;
using UnityEngine;
[DefaultExecutionOrder(-100)]
public class AppStateManager : MonoBehaviour
{
    public static AppStateManager Instance { get; private set; }  //Singleton instance
    /*
     * API:
     * Set state:
     * statusManager.SetHome();
     * statusManager.SetGameplay();
     * statusManager.SetLoading(); or statusManager.ForceSetState(AppState.YourState)
     * 
     * Toggle overlays independently:
     * statusManager.SetOverlay("Pause", true); // show
     * statusManager.SetOverlay("Pause", false); // hide
     * bool visible = statusManager.IsOverlayVisible("Pause");
     * Sync from current screen activeness (optional, manual):
     * statusManager.RefreshFromPages(); // scans screen bindings and sets state to the first active screen
     *
     * Events:
     * OnStateChanged(oldState, newState) fires on every state update.
     *
     */
    public enum AppState
    {
        Unknown = 0,
        Splash,
        SplashOnLoad,
        Tutorial,
        TutorialOnLoad,
        Home,
        Gameplay,
        Settings,
        Credits,
        Loading,
        GameOver,


    }

    [Header("Persistence")]
    [SerializeField] private bool _dontDestroyOnLoad = true;

    [Header("UI Roots")]
    [SerializeField] private Transform screensParent;
    [SerializeField] private Transform overlaysParent;

    [Serializable]
    public sealed class ScreenBinding
    {
        public string name;
        public AppState state;
        public GameObject prefab;
        [NonSerialized] public GameObject instance;
    }

    [SerializeField]
    private ScreenBinding[] screens = Array.Empty<ScreenBinding>();
    [Serializable]
    public sealed class OverlayBinding
    {
        public string key; // e.g., Pause, HUD, Warning
        public GameObject prefab;
        [NonSerialized] public GameObject instance;
    }

    [SerializeField]
    private OverlayBinding[] overlays = Array.Empty<OverlayBinding>();

    // Current state
    public AppState CurrentState { get; private set; } = AppState.Unknown;

    // Events
    public event Action<AppState, AppState> OnStateChanged; // (oldState, newState)

    private void Awake()
    {
        // Singleton guard
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        RefreshFromPages();

    }

    // Public: high-level state setters for single-scene flows

    public void SetSplash() => SetState(AppState.Splash);
    public void SetSplashOnLoad() => SetState(AppState.SplashOnLoad);
    public void SetTutorial() => SetState(AppState.Tutorial);
    public void SetTutorialOnLoad() => SetState(AppState.TutorialOnLoad);
    public void SetHome() => SetState(AppState.Home);
    public void SetGameplay() => SetState(AppState.Gameplay);
    public void SetGameOver() => SetState(AppState.GameOver);

    public void SetSettings() => SetState(AppState.Settings);
    public void SetCredits() => SetState(AppState.Credits);
    public void SetLoading() => SetState(AppState.Loading);

    public void ForceSetState(AppState state) => SetState(state);

    private void EnsureScreenInstantiated(ScreenBinding b)
    {
        if (b == null || b.instance != null) return;
        if (b.prefab == null) return;
        var p = screensParent;
        b.instance = Instantiate(b.prefab, p);
        b.instance.name = b.prefab.name;
        b.instance.SetActive(false);
        b.instance.transform.SetAsLastSibling();
    }
    private void EnsureOverlayInstantiated(OverlayBinding o)
    {
        if (o == null || o.instance != null) return;
        if (o.prefab == null) return;
        var p = overlaysParent;
        o.instance = Instantiate(o.prefab, p);
        o.instance.name = o.prefab.name;
        o.instance.SetActive(false);
    }

    private GameObject GetScreenRoot(ScreenBinding b)
    {
        return b?.instance;
    }

    private GameObject GetOverlayRoot(OverlayBinding o)
    {
        return o?.instance;
    }

    private void SetState(AppState newState)
    {
        if (newState == CurrentState) return;

        var old = CurrentState;
        CurrentState = newState;

        ApplyPageVisibilityForState(newState);

        OnStateChanged?.Invoke(old, newState);
    }
    public void RefreshFromPages()
    {
        if (CurrentState == AppState.Unknown)
            SetState(AppState.Splash);
    }
    private void ApplyPageVisibilityForState(AppState state)
    {
        foreach (var b in screens)
        {
            if (b == null) continue;
            bool shouldBeActive = b.state.Equals(state);
            if (shouldBeActive)
            {
                EnsureScreenInstantiated(b);
                if (b.instance != null && !b.instance.activeSelf)
                    b.instance.SetActive(true);
            }
            else
            {
                if (b.instance != null && b.instance.activeSelf)
                    Destroy(b.instance);
            }
        }
    }
    public void ShowOverlay(string key)
    {
        SetOverlay(key, true);
    }

    public void HideOverlay(string key)
    {
        SetOverlay(key, false);
    }

    public void SetOverlay(string key, bool visible)
    {
        if (string.IsNullOrEmpty(key)) return;
        foreach (var o in overlays)
        {
            if (o == null) continue;
            if (string.Equals(o.key, key, StringComparison.Ordinal))
            {
                if (visible)
                {
                    EnsureOverlayInstantiated(o);
                    if (o.instance != null && !o.instance.activeSelf)
                        o.instance.SetActive(true);
                }
                else
                {
                    if (o.instance != null && o.instance.activeSelf)
                        Destroy(o.instance);
                }
                return;
            }
        }
    }
    public bool IsOverlayVisible(string key)
    {
        if (string.IsNullOrEmpty(key)) return false;
        foreach (var o in overlays)
        {
            if (o == null) continue;
            if (string.Equals(o.key, key, StringComparison.Ordinal))
            {
                return o.instance != null && o.instance.activeSelf;
            }
        }
        return false;
    }
}
