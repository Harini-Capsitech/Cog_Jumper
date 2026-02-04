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

    public event Action OnLanguageChanged;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (localizationJson == null)
        {
            Debug.LogError("Localization JSON not assigned!");
            return;
        }

        localizedData =
            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(
                localizationJson.text
            );
    }
    public void SetEnglish() => SetLanguage("en");
    public void SetPortuguese() => SetLanguage("pt-BR");
    public void SetIndonesian() => SetLanguage("id");


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
