using UnityEngine;

public class ppbutton : MonoBehaviour
{
    public void OnPrivacyPolicyClicked()
    {
        Application.OpenURL("https://www.thegamewise.com/privacy-policy/");
    }
}
