using UnityEngine;
using UnityEngine.EventSystems;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance;
    private bool blockInputOneFrame;

    private bool isPaused;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // 🔒 Disable EventSystem input
        if (EventSystem.current != null)
            EventSystem.current.enabled = false;

        // 🔇 Pause audio
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (EventSystem.current != null)
            EventSystem.current.enabled = true;

        AudioListener.pause = false;

        // 🔒 Block input for next frame
        blockInputOneFrame = true;
    }

    public bool ShouldBlockInput()
    {
        if (blockInputOneFrame)
        {
            blockInputOneFrame = false;
            return true;
        }
        return false;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
