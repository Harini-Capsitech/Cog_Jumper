using System.Collections;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField] private GameObject gameLogicPrefab;
    private GameObject gameLogic;

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

        // to stop sound after GAME starts .
        SoundManager.Instance.StopSfx();

        

        Time.timeScale = 1f;

        if (gameLogic != null)

        {
            Destroy(gameLogic);
            gameLogic = null;
        }

        AppStateManager.Instance.SetHome();
    }

<<<<<<< HEAD
=======
    // ---------------- SETTINGS ----------------

>>>>>>> 7ae73a8beeba362d2dd6f50f53126be8eda42c42
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
<<<<<<< HEAD

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
=======
>>>>>>> 7ae73a8beeba362d2dd6f50f53126be8eda42c42
}
