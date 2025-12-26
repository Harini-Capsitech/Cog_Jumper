using System.Collections;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField] private GameObject gameLogicPrefab;
    private GameObject gameLogic;

    public static AppManager instance;

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
        if (gameLogic != null)
        {
            Destroy(gameLogic);
            gameLogic = null;
        }

        AppStateManager.Instance.SetGameOver();
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
}
