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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    
    public void PlayJump()
    {
        if (jumpSfx != null)
            sfxSource.PlayOneShot(jumpSfx);
    }

    public void PlayGameOver()
    {
        if (gameOverSfx != null)
            sfxSource.PlayOneShot(gameOverSfx);
    }

    public void PlaySfx()
    {
        if (sfxClip != null && !sfxSource.isPlaying)
            sfxSource.PlayOneShot(sfxClip);
    }
    public void StopSfx()
    {
        if (sfxSource.isPlaying)
            sfxSource.Stop();
    }

}
