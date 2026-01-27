//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Events;

//public class AudioManager : MonoBehaviour
//{
//    /*
//        AudioManager — How to Use
//        -------------------------
//        Central system for all MUSIC and SFX. Other scripts never touch
//        AudioSources directly—only call the methods below.

//        1. Basic Usage
//        --------------
//        // Play music (optional loop)
//        AudioManager.Instance.PlayMusic("MainMenu", true);

//        // Stop specific music
//        AudioManager.Instance.StopMusic("MainMenu");

//        // Play SFX (auto-destroy if not looping)
//        AudioManager.Instance.PlaySfx("Click");

//        // Stop specific SFX
//        AudioManager.Instance.StopSfx("WindLoop");

//        2. Mute Controls (for settings UI)
//        ----------------------------------
//        AudioManager.Instance.MuteMusic(true/false);
//        AudioManager.Instance.MuteSfx(true/false);

//        3. Global Pause (for pause menu)
//        ---------------------------------
//        AudioManager.Instance.PauseAll();
//        AudioManager.Instance.ResumeAll();

//        4. Notes
//        --------
//        - AudioSources are created automatically and cleaned up as needed.
//        - Multiple music tracks can play unless you stop them.
//        - SFX that aren't looping remove themselves after finishing.
//        - All audio objects are parented under AudioManager.
//    */

//    [Header("Sound Collections")]
//    public Sound[] musicSounds;

//    private Dictionary<string, Sound> musicDict;


//    // Pool of all active AudioSources
//    private readonly List<AudioSource> activeMusic = new();

//    public static AudioManager Instance { get; private set; }
//    public bool musicIsInMute { get; private set; }
//    public bool vibrationIsInMute { get; private set; }


//    private void Awake()
//    {
//        CheckInstance();
//        InitializeDictionaries();
//        vibrationIsInMute = false;
//    }

//    private void CheckInstance()
//    {
//        // Singleton pattern
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        DontDestroyOnLoad(gameObject);
//    }

//    private void InitializeDictionaries()
//    {
//        musicDict = new Dictionary<string, Sound>();

//        foreach (var s in musicSounds)
//            if (!musicDict.ContainsKey(s.name))
//                musicDict.Add(s.name, s);
//    }

//    #region MUSIC

//    public void PlayMusic(string name, bool loop = false)
//    {
//        if (!musicDict.TryGetValue(name, out Sound s) || s.clip == null)
//        {
//            Debug.LogWarning("Music not found: " + name);
//            return;
//        }

//        AudioSource src = CreateSource(s, loop, musicIsInMute);
//        activeMusic.Add(src);
//        if (!loop && src)
//            StartCoroutine(DelayCallback(src.clip.length, () =>
//            {
//                if (src)
//                    Destroy(src.gameObject);

//            }));
//    }
//    private IEnumerator DelayCallback(float delay, UnityAction action)
//    {
//        yield return new WaitForSeconds(delay);
//        action?.Invoke();
//    }

//    public void StopMusic(string name)
//    {
//        if (!musicDict.TryGetValue(name, out Sound s)) return;

//        CleanupSources(activeMusic, s.clip);
//    }

//    public void MuteMusic(bool value)
//    {
//        musicIsInMute = value;

//        foreach (var src in activeMusic)
//            if (src != null)
//                src.mute = value;
//    }

//    #endregion



//    #region VIBRATION

//    public void Vibrate()
//    {
//        if (!vibrationIsInMute)
//        {
//            VibrationManager.Vibrate(VibrationType.Light);
//        }
//    }

//    public void MuteVibration(bool value)
//    {
//        vibrationIsInMute = value;
//    }

//    #endregion

//    #region Global Controls

//    public void PauseAll()
//    {
//        foreach (var src in activeMusic) src?.Pause();
//    }

//    public void ResumeAll()
//    {
//        foreach (var src in activeMusic) src?.UnPause();
//    }

//    #endregion

//    #region Helpers

//    private AudioSource CreateSource(Sound sound, bool loop, bool mute)
//    {
//        GameObject go = new GameObject("Audio_" + sound.name);
//        go.transform.SetParent(transform);

//        AudioSource src = go.AddComponent<AudioSource>();
//        src.clip = sound.clip;
//        src.volume = sound.volume;
//        src.loop = loop;
//        src.mute = mute;
//        src.Play();

//        return src;
//    }

//    private IEnumerator DestroyAfterPlay(AudioSource src, List<AudioSource> list)
//    {
//        yield return new WaitWhile(() => src != null && src.isPlaying);

//        if (src != null)
//        {
//            list.Remove(src);
//            Destroy(src.gameObject);
//        }
//    }

//    private void CleanupSources(List<AudioSource> list, AudioClip clip)
//    {
//        for (int i = list.Count - 1; i >= 0; i--)
//        {
//            if (list[i] != null && list[i].clip == clip)
//            {
//                Destroy(list[i].gameObject);
//                list.RemoveAt(i);
//            }
//        }
//    }

//    #endregion
//}