using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    public void English()
    {
        LocalizationManager.Instance?.SetLanguage("en");
    }

    public void Portuguese()
    {
        LocalizationManager.Instance?.SetLanguage("pt");
    }

    public void Indonesian()
    {
        LocalizationManager.Instance?.SetLanguage("id");
    }

    public void Arabic()
    {
        LocalizationManager.Instance?.SetLanguage("ar");
    }

    public void Spanish()
    {
        LocalizationManager.Instance?.SetLanguage("es");
    }
    public void French()
    {
        LocalizationManager.Instance?.SetLanguage("fr");
    }
}
