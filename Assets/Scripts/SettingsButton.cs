using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public void OnSettingsClicked()
    {
        AppManager.instance.OpenSettings();
    }
}
