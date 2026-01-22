using UnityEngine;
using UnityEngine.EventSystems;

public class SaveMeButtonUI : MonoBehaviour
{
    private void Update()
    {
       
    }
    public void OnSaveMeClicked()
    {
        Debug.Log("save me activated");

        if (GameFlowController.Instance == null)
            return;


        Debug.Log("activated");
        // 1️⃣ First check if normal SaveMe is allowed (time-based life)
        if (GameFlowController.Instance.CanUseSaveMe())
        {
            Debug.Log("Using normal SaveMe");
            GameFlowController.Instance.SaveMe();
            return;
        }

        // 2️⃣ If life exhausted → try Rewarded Ad
        Debug.Log("Normal SaveMe exhausted, trying rewarded ad");

        if (GoogleMobileAdsDemoScript.Instance != null &&
            GoogleMobileAdsDemoScript.Instance.IsRewardedReady())
        {
            GoogleMobileAdsDemoScript.Instance.ShowRewardedForStraighten(() =>
            {
                Debug.Log("Rewarded ad completed → granting SaveMe");
               

                
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad not ready");
        }
    }
}
