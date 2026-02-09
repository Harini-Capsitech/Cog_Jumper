using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager Instance;

    private const string PREF_KEY = "VIBRATION_ENABLED";
    public bool IsEnabled { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Default: OFF
        IsEnabled = PlayerPrefs.GetInt(PREF_KEY, 0) == 1;
    }

    public void Toggle()
    {
        IsEnabled = !IsEnabled;
        PlayerPrefs.SetInt(PREF_KEY, IsEnabled ? 1 : 0);
        PlayerPrefs.Save();

        // Feedback when turning ON
        if (IsEnabled)
            Vibrate();
    }

    public void Vibrate()
    {
        if (!IsEnabled) return;

#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }
}
