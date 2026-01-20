using UnityEngine;
using UnityEngine.UI;

public class x2ButtonUI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int scoreThreshold = 50;

    [Header("UI")]
    [SerializeField] private Button x2Button;

    private bool popupShown = false;
    private bool x2Used = false;

    void Start()
    {
        //if (x2Button != null)
        //    x2Button.gameObject.SetActive(false);
    }

    void Update()
    {
    //    if (x2Used) return;
    //    if (GameFlowController.Instance == null) return;

    //    int currentScore = GameFlowController.Instance.GetScore();

    //    if (currentScore >= scoreThreshold && !popupShown)
    //    {
    //        popupShown = true;

    //        Debug.Log("Score reached 200 → showing x2 popup");

    //        PerfectPopup.Instance?.Show();

    //        if (x2Button != null)
    //            x2Button.gameObject.SetActive(true);
    //    }
    }

    // Called from Button OnClick()
    public void OnX2Clicked()
    {
        if (x2Used) return;

        Debug.Log("X2 button clicked → showing interstitial");

        GoogleMobileAdsDemoScript.Instance.ShowInterstitialOnRestart(() =>
        {
            ApplyX2();
        });
    }

    private void ApplyX2()
    {
        if (x2Used) return;

        x2Used = true;

        Debug.Log("X2 reward applied");

        GameFlowController.Instance.ApplyScoreMultiplierOnce(2);

        if (x2Button != null)
            x2Button.gameObject.SetActive(false);
    }
    public void TryActivateX2(int currentScore)
    {
        if (x2Used) return;
        if (popupShown) return;

        if (currentScore >= scoreThreshold)
        {
            popupShown = true;

            Debug.Log("X2 unlocked");

            PerfectPopup.Instance?.Show();

            if (x2Button != null)
                x2Button.gameObject.SetActive(true);
        }
    }

}
