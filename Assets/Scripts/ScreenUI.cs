using UnityEngine;

public class ScreenUI : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
       AppManager.instance.StartGame();
    }
}
