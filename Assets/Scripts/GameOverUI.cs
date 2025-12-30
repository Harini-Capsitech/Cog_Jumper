using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    public GameObject panel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(int score, int bestScore)
    {
        panel.SetActive(true);
        scoreText.text = score.ToString();
        bestScoreText.text = bestScore.ToString();
    }

    public void RestartGame()
    {

        AppManager.instance.RestartGame();
    }
    public void Home()
    {
        AppManager.instance.GoToHome();
    }
    public void Hide()
    {
        panel.SetActive(false);
    }
}


