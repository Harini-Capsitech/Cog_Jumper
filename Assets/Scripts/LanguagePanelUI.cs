using UnityEngine;

public class LanguagePanelUI : MonoBehaviour
{
    [SerializeField] public GameObject lanPanel;

    

    public void OpenLanguagePanel()
    {
        Debug.Log("LANGUAGE BUTTON CLICKED");
        lanPanel.SetActive(true);
    }

    public void CloseLanguagePanel()
    {
        lanPanel.SetActive(false);
    }
}
