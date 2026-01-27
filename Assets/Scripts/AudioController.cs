//using UnityEngine;
//using UnityEngine.UI;

//public class AudioController : MonoBehaviour
//{
//    [Header("Music UI")]
//    [SerializeField] Sprite[] musicIcon;
//    [SerializeField] Image musicImage;

//    //[Header("SFX UI")]
//    //[SerializeField] Sprite[] sfxIcon;     

//    //[SerializeField] Image sfxImage; 

//    [Header("Vibration UI")]
//    [SerializeField] Sprite[] vibrationIcon;
//    [SerializeField] Image vibrationImage;

//    private Button musicButton;

//    private Button sfxButton;
//    private Button vibrationButton;

//    private void Start()
//    {
//        if (musicImage != null)
//        {
//            musicButton = musicImage.GetComponent<Button>();
//            musicButton.onClick.AddListener(ToggleMusic);
//            UpdateMusicIcon();
//        }
//        if (vibrationImage != null)
//        {
//            vibrationButton = vibrationImage.GetComponent<Button>();
//            vibrationButton.onClick.AddListener(ToggleVibration);
//            UpdateVibrationIcon();
//        }
//    }

//    private void ToggleMusic()
//    {
//        AudioManager.Instance.MuteMusic(!AudioManager.Instance.musicIsInMute);
//        UpdateMusicIcon();
//    }


//    private void ToggleVibration()
//    {
//        AudioManager.Instance.MuteVibration(!AudioManager.Instance.vibrationIsInMute);
//        UpdateVibrationIcon();
//    }

//    public void UpdateMusicIcon()
//    {
//        musicImage.sprite = AudioManager.Instance.musicIsInMute ? musicIcon[1] : musicIcon[0];
//    }


//    public void UpdateVibrationIcon()
//    {

//        vibrationImage.sprite = AudioManager.Instance.vibrationIsInMute ? vibrationIcon[1] : vibrationIcon[0];
//    }
//}
