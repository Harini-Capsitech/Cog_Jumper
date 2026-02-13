using UnityEngine;

public class ScreenUI : MonoBehaviour
{
    private void OnEnable()
    {
        //  Play gameplay music when this panel becomes visible
        
    }
    public void OnPlayButtonClicked()
    {

        //  Stop music immediately when Play is clicked

        AppManager.instance.StartGame();
    }
}
