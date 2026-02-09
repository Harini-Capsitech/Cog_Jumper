using UnityEngine;

public class LanguagePanelUI : MonoBehaviour
{
    [SerializeField] private GameObject lanPanel;


    //private void Awake()
    //{
    //    lanPanel.SetActive(false);
    //}
    public void OpenLanguagePanel()
    {
        lanPanel.SetActive(true);
        Debug.Log("LANGUAGE BUTTON CLICKED");
    }

    public void CloseLanguagePanel()
    {
        lanPanel.SetActive(false);
    }
}
