using UnityEngine;
using UnityEngine.UI;

public class PausePopupUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button homeButton;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackClicked);
        resumeButton.onClick.AddListener(OnResumeClicked);
        homeButton.onClick.AddListener(OnHomeClicked);
    }

    private void OnDisable()
    {
        backButton.onClick.RemoveListener(OnBackClicked);
        resumeButton.onClick.RemoveListener(OnResumeClicked);
        homeButton.onClick.RemoveListener(OnHomeClicked);
    }

    public void OnResumeClicked()
    {
        AppManager.instance.ResumeGame();
    }

    public void OnHomeClicked()
    {
        
        AppManager.instance.ExitGameToHomeFromPause();
    }
    public void OnBackClicked()
    {
        Debug.Log("pause!!!!");
        AppManager.instance.ClosePause();
    }
}
