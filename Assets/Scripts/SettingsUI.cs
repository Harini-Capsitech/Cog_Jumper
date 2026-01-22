using UnityEngine;
using UnityEngine.UI;
public enum SettingsType
{
    home,
    play
}
public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private SettingsType st;
    // [SerializeField] private Button privacyPolicyButton;

    private void OnEnable()
    {
        backButton.onClick.AddListener(OnBackClicked);
        // privacyPolicyButton.onClick.AddListener(OnPrivacyPolicyClicked);
    }

    private void OnDisable()
    {
        backButton.onClick.RemoveListener(OnBackClicked);
        // privacyPolicyButton.onClick.RemoveListener(OnPrivacyPolicyClicked);
    }

    private void OnBackClicked()
    {
        AppManager.instance.CloseSettings(st);
    }

    void OnPrivacyPolicyClicked()
    {
        Application.OpenURL("http://www.thegamewise.com/privacy-policy/");
    }
}
