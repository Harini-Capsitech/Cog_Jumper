using System.Collections;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField] private GameObject gameLogicPrefab;
    private GameObject gameLogic;
    public bool IsPaused { get; private set; } = false;
    public static AppManager instance;
    public bool isSaveMeActive = false;
    private Vector3 camStartPos;
    private void Awake()
    {
       
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        camStartPos = Camera.main.transform.position;
        StartCoroutine(ShowLoading());
    }

    IEnumerator ShowLoading()
    {
        AppStateManager.Instance.SetLoading();
        yield return new WaitForSeconds(2f);
        AppStateManager.Instance.SetHome();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        Camera.main.transform.position = camStartPos;

        if (gameLogic != null)
        {
            Destroy(gameLogic);
            gameLogic = null;
        }

        gameLogic = Instantiate(gameLogicPrefab);

        AppStateManager.Instance.SetGameplay();
    }

    public void GameOver()
    {
        AppStateManager.Instance.SetGameOver();
        if (isSaveMeActive)
            return;
        if (gameLogic != null)
        {
            Destroy(gameLogic);
            gameLogic = null;
        }

       
    }

    public void RestartGame()
    {

        SoundManager.Instance.PlaySfx();
        Time.timeScale = 1f;
        this.StartGame();
        Camera.main.transform.position = camStartPos;

        StartGame();
    }

    public void GoToHome()
    {

        
        SoundManager.Instance.StopSfx();

        

        Time.timeScale = 1f;

        if (gameLogic != null)

        {
            Destroy(gameLogic);
            gameLogic = null;
        }

        GoogleMobileAdsDemoScript.Instance.ShowInterstitialOnRestart(() =>
        {

        });
        AppStateManager.Instance.SetHome();
    }

    public void OpenSettings()
    {
        SoundManager.Instance.PlaySfx();
        Time.timeScale = 0f; // pause game
        AppStateManager.Instance.SetSettings();
    }

    public void CloseSettings(SettingsType st)
    {
        if (st == SettingsType.home)
        {
            SoundManager.Instance.PlaySfx();
            Time.timeScale = 1f;
            AppStateManager.Instance.SetHome();
        }
        if (st == SettingsType.play)
        {
            SoundManager.Instance.PlaySfx();
            Time.timeScale = 1f;
            AppStateManager.Instance.SetGameplay();
        }
    }

    public void PauseGame()
    {
        if (IsPaused) return;

        IsPaused = true;

        SoundManager.Instance.PlaySfx();

        Time.timeScale = 0f;
      
        AppStateManager.Instance.SetPause();
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        IsPaused = false;

        SoundManager.Instance.PlaySfx();

        Time.timeScale = 1f;

        AppStateManager.Instance.SetGameplay();
    }

    public void ExitGameToHomeFromPause()
    {
        IsPaused = false;

        SoundManager.Instance.PlaySfx();

        Time.timeScale = 1f;

        if (gameLogic != null)
        {
            Destroy(gameLogic);
            gameLogic = null;
        }

        GoogleMobileAdsDemoScript.Instance.ShowInterstitialOnRestart(() =>
        {

        });
        AppStateManager.Instance.SetHome();
    }

    public void ClosePause()
    {
        if (!IsPaused) return;

        IsPaused = false;

        SoundManager.Instance.PlaySfx();

        Time.timeScale = 1f;
        Debug.Log("pause closed");
        //
        AppStateManager.Instance.SetGameplay();
    }

    public void OpenLanguagePanel()
    {
        SoundManager.Instance.PlaySfx();
        AppStateManager.Instance.ShowOverlay("Language");
    }
    public void disableGameLogic()
    {
        if(gameLogic != null)
        {
            gameLogic.SetActive(false);
        }
    }
    public void enableGameLogic()
    {
        if (gameLogic != null)
        {
            gameLogic.SetActive(true);
        }
    }
}
