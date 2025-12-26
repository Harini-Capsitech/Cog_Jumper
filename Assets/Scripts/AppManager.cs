using System.Collections;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    
    [SerializeField] private GameObject GameLogicPrefab;
    private GameObject GameLogic;

    public static AppManager instance;

    [SerializeField] private Vector3 camStartPos;
    void Start()
    {
        instance = this;
        StartCoroutine(ShowLoading());
        camStartPos = Camera.main.transform.position;
    }

    void Update()
    {
        
    }

    IEnumerator ShowLoading()
    {
        AppStateManager.Instance.SetLoading();
        yield return new WaitForSeconds(2f);
        AppStateManager.Instance.SetHome();
    }

    public void StartGame()
    {
        if (GameLogic == null)
        {
            GameLogic = Instantiate(GameLogicPrefab);
        }
        else
        {
            GameLogic.SetActive(true);
        }
        AppStateManager.Instance.SetGameplay();
    }

    public void GameOver()
    {
        if (GameLogic != null)
        {
            GameLogic.SetActive(false);
            Destroy(GameLogic);
        }

        AppStateManager.Instance.SetGameOver();
    }

    public void RestartGame()
    {
        SoundManager.Instance.PlaySfx();
        Time.timeScale = 1f;
        this.StartGame();
        Camera.main.transform.position = camStartPos;
    }

    
    public void GoToHome()
    {
        // to stop sound after GAME starts .
        SoundManager.Instance.StopSfx();

        if (GameLogic != null)
        {
            Destroy(GameLogic);
            GameLogic = null;
        }

        Time.timeScale = 1f;
        AppStateManager.Instance.SetHome();
    }
}
