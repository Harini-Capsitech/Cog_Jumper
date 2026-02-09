using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    [SerializeField] private TextAsset localizationJson;


    Dictionary<string, Dictionary<string, string>> localizedData;
    string currentLangCode = "en";
    public string CurrentLanguage => currentLangCode;
    public event Action OnLanguageChanged;

    void Awake()
    {
        if (Instance != null)
        {
            //Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        if (localizationJson == null)
        {
            Debug.LogError("Localization JSON not assigned!");
            return;
        }

        localizedData =
            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(
                localizationJson.text
            );


        currentLangCode = PlayerPrefs.GetString("LANGUAGE_CODE", "en");
    }
    public void SetEnglish() => SetLanguage("en");
    public void SetFrench() => SetLanguage("fr");
    public void SetPortuguese() => SetLanguage("pt");
    public void SetArabic() => SetLanguage("ar");
    public void SetIndonesian() => SetLanguage("id");
    public void SetSpanish() => SetLanguage("es");


    void LoadJson()
    {
        TextAsset json = Resources.Load<TextAsset>("Localization/Data/localization.json");

        if (json == null)
        {
            Debug.LogError("Localization JSON not found!");
            return;
        }

        localizedData =
            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json.text);
    }

    public void SetLanguage(string langCode)
    {
        if (currentLangCode == langCode)
            return;

        currentLangCode = langCode;


        PlayerPrefs.SetString("LANGUAGE_CODE", langCode);
        PlayerPrefs.Save();

        OnLanguageChanged?.Invoke();
    }


    public string GetText(string key)
    {
        if (!localizedData.TryGetValue(key, out var langMap))
            return $"#{key}";

        if (!langMap.TryGetValue(currentLangCode, out var value))
            return $"#{key}_{currentLangCode}";

        return value;
    }
}
