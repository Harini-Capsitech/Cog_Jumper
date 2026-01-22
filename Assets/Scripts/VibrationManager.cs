//using System;
//using UnityEngine;

//public enum VibrationType
//{
//    Light,
//    Medium,
//    Heavy,
//    Success,
//    Error,
//    Long,
//    CustomPattern
//}

//public class VibrationManager : MonoBehaviour
//{
//#if UNITY_ANDROID && !UNITY_EDITOR
//    private static AndroidJavaObject vibrator;
//    private static AndroidJavaObject context;
//#endif

//    void Awake()
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        if (vibrator == null)
//        {
//            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//            context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//            vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");
//        }
//#endif
//    }

//    // Public method to call from anywhere
//    public static void Vibrate(VibrationType type)
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        if (vibrator == null) return;

//        long milliseconds = 100;
//        int amplitude = 128; // Range: 1�255
//        long[] pattern = null;

//        switch (type)
//        {
//            case VibrationType.Light:
//                milliseconds = 50;
//                amplitude = 80;
//                break;

//            case VibrationType.Medium:
//                milliseconds = 120;
//                amplitude = 150;
//                break;

//            case VibrationType.Heavy:
//                milliseconds = 250;
//                amplitude = 255;
//                break;

//            case VibrationType.Success:
//                pattern = new long[] { 0, 50, 50, 100 };
//                break;

//            case VibrationType.Error:
//                pattern = new long[] { 0, 100, 50, 200, 50, 300 };
//                break;

//            case VibrationType.Long:
//                milliseconds = 600;
//                amplitude = 200;
//                break;

//            case VibrationType.CustomPattern:
//                pattern = new long[] { 0, 100, 100, 200, 100, 300 }; // Example pattern
//                break;
//        }

//        try
//        {
//            AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
//            AndroidJavaObject vibrationEffect;

//            if (pattern != null)
//            {
//                vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
//                    "createWaveform", pattern, -1);
//            }
//            else
//            {
//                vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
//                    "createOneShot", milliseconds, amplitude);
//            }

//            vibrator.Call("vibrate", vibrationEffect);
//        }
//        catch
//        {
//            // Fallback for older Android versions
//            if (pattern != null)
//                vibrator.Call("vibrate", pattern, -1);
//            else
//                vibrator.Call("vibrate", milliseconds);
//        }

//#elif UNITY_IOS && !UNITY_EDITOR
//        // iOS fallback (default short vibration)
//        Handheld.Vibrate();
//#else
//        Debug.Log($"[Vibration] Simulated vibration: {type}");
//#endif
//    }

//    public static void Cancel()
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        if (vibrator != null)
//            vibrator.Call("cancel");
//#endif
//    }
//}
