using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip sfxClip;
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private AudioClip gameOverSfx;

    public bool IsMusicOn { get; private set; } = true;

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

    public void ToggleMusic()
    {
        IsMusicOn = !IsMusicOn;

        if (IsMusicOn)
            PlaySfx();
        else
            StopSfx();
    }

    public void PlaySfx()
    {
        if (IsMusicOn && sfxClip != null && !sfxSource.isPlaying)
            sfxSource.PlayOneShot(sfxClip);
    }

    public void StopSfx()
    {
        sfxSource.Stop();
    }

    public void PlayJump()
    {
        if (IsMusicOn && jumpSfx != null)
            sfxSource.PlayOneShot(jumpSfx);
    }

    public void PlayGameOver()
    {
        if (IsMusicOn && gameOverSfx != null)
            sfxSource.PlayOneShot(gameOverSfx);
    }
}
