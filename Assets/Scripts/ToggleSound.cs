using UnityEngine;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour
{
    [SerializeField] private Image soundIcon;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;

    private void OnEnable()
    {
        UpdateIcon();
    }

    public void OnButtonClicked()
    {
        SoundManager.Instance.ToggleMusic();
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (SoundManager.Instance.IsMusicOn)
            soundIcon.sprite = musicOnSprite;
        else
            soundIcon.sprite = musicOffSprite;
    }
}
