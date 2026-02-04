using UnityEngine;

public class PauseButtonUI : MonoBehaviour
{
    public void OnPauseClicked()
    {
        AppManager.instance.PauseGame();
    }
}
