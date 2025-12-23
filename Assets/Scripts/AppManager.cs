using System.Collections;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
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

        // Show Game Over UI
        AppStateManager.Instance.SetGameOver();
    }

    public void RestartGame()
    {
        

        // Reset time
        Time.timeScale = 1f;
        this.StartGame();
        Camera.main.transform.position = camStartPos;
    }

    // 🏠 Optional: Go back to Home
    public void GoToHome()
    {
        if (GameLogic != null)
        {
            Destroy(GameLogic);
            GameLogic = null;
        }

        Time.timeScale = 1f;
        AppStateManager.Instance.SetHome();
    }
}
