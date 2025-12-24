using UnityEngine;

public class ScreenUI : MonoBehaviour
{
    private void OnEnable()
    {
        //  Play gameplay music when this panel becomes visible
        SoundManager.Instance.PlaySfx();
    }
    public void OnPlayButtonClicked()
    {

        //  Stop music immediately when Play is clicked
        SoundManager.Instance.StopSfx();

        AppManager.instance.StartGame();
    }
}
