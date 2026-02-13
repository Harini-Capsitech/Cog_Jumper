//using UnityEngine;

//public class SoundManager : MonoBehaviour
//{
//    public static SoundManager Instance;

//    [Header("Audio Source")]
//    [SerializeField] private AudioSource sfxSource;
//    [SerializeField] private AudioSource musicSource;

//    [Header("SFX Clips")]
//    [SerializeField] private AudioClip sfxClip;
//    [SerializeField] private AudioClip jumpSfx;
//    [SerializeField] private AudioClip gameOverSfx;

//    public bool IsMusicOn { get; private set; } = true;
//    public bool IsSoundOn { get; private set; } = true;
//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        DontDestroyOnLoad(gameObject);
//    }

//    public void ToggleMusic()
//    {
//        IsMusicOn = !IsMusicOn;

//        if (IsMusicOn)
//            PlaySfx();
//        else
//            StopMusic();
//    }

//    public void ToggleSound()
//    {
//        IsSoundOn = !IsSoundOn;

//        if (IsSoundOn)
//            PlaySfx();

//    }
//    public void PlaySfx()
//    {
//        if (IsMusicOn && sfxClip != null && !sfxSource.isPlaying)
//            sfxSource.PlayOneShot(sfxClip);
//    }

//    public void PlayMusic()
//    {
//        if (IsMusicOn && sfxClip != null && !musicSource.isPlaying)
//            musicSource.clip = sfxClip;
//            musicSource.loop = true;
//            musicSource.Play();
//    }

//    public void StopMusic()
//    {
//        sfxSource.Pause();
//    }

//    public void PlayJump()
//    {
//        if (IsMusicOn && jumpSfx != null)
//            sfxSource.PlayOneShot(jumpSfx);
//    }

//    public void PlayGameOver()
//    {
//        if (IsMusicOn && gameOverSfx != null)
//            sfxSource.PlayOneShot(gameOverSfx);
//    }
//}

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Clips")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private AudioClip gameOverSfx;
    [SerializeField] private AudioClip buttonClickSfx;

    public bool IsMusicOn { get; private set; } = true;
    public bool IsSoundOn { get; private set; } = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        PlayMusic();
    }

    // ================= MUSIC =================

    public void ToggleMusic()
    {
        IsMusicOn = !IsMusicOn;

        if (IsMusicOn)
            PlayMusic();
        else
            StopMusic();
    }

    public void PlayMusic()
    {
        if (!IsMusicOn || musicClip == null) return;

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // ================= SFX =================

    public void ToggleSound()
    {
        IsSoundOn = !IsSoundOn;
    }

    public void PlayJump()
    {
        if (!IsSoundOn || jumpSfx == null) return;
        sfxSource.PlayOneShot(jumpSfx);
    }

    public void PlayGameOver()
    {
        if (!IsSoundOn || gameOverSfx == null) return;
        sfxSource.PlayOneShot(gameOverSfx);
    }

    public void PlayButtonClick()
    {
        if (!IsSoundOn || buttonClickSfx == null) return;
        sfxSource.PlayOneShot(buttonClickSfx);
    }
    public void PlaySfx()
    {
        if (!IsSoundOn) return;

        if (buttonClickSfx != null)
            sfxSource.PlayOneShot(buttonClickSfx);
    }

}
