using ArabicSupport;
using TMPro;
using UnityEngine;

public class LocalizedTMPText : MonoBehaviour
{
    [SerializeField] private string key;

    [Header("Fonts")]
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset arabicFont;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        string lang = LocalizationManager.Instance.CurrentLanguage;
        string value = LocalizationManager.Instance.GetText(key);

        switch (lang)
        {
            case "ar":
                ApplyArabic(value);
                break;

            default:
                ApplyLTR(value);
                break;
        }
    }

    private void ApplyArabic(string value)
    {
        text.font = arabicFont;
        text.isRightToLeftText = true;
        text.alignment = TextAlignmentOptions.Center;
        text.text = ArabicFixer.Fix(value, true, true);
    }

    private void ApplyLTR(string value)
    {
        text.font = defaultFont;
        text.isRightToLeftText = false;
        text.alignment = TextAlignmentOptions.Center;
        text.text = value;
    }
}
