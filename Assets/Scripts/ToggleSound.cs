using UnityEngine;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour
{
    [SerializeField] private Image soundIcon;
    [SerializeField] private Image sfxIcon;

    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    private void OnEnable()
    {
        UpdateIcon();
        UpdateSoundIcon();
    }

    public void OnButtonClicked()
    {
        SoundManager.Instance.ToggleMusic();
        UpdateIcon();
    }
    public void OnButtonsfxClicked()
    {
        SoundManager.Instance.ToggleSound();
        UpdateSoundIcon();
    }

    private void UpdateIcon()
    {
        if (SoundManager.Instance.IsMusicOn)
            soundIcon.sprite = musicOnSprite;
        else
            soundIcon.sprite = musicOffSprite;
    }

    private void UpdateSoundIcon()
    {
        if (SoundManager.Instance.IsSoundOn)
            sfxIcon.sprite = soundOnSprite;
        else
            sfxIcon.sprite = soundOffSprite;
    }
}
