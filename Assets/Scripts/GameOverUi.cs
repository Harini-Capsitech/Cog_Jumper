//using TMPro;

//using UnityEngine;

//using UnityEngine.SceneManagement;

//public class GameOverUI : MonoBehaviour

//{

//    public static GameOverUI Instance;

//    public GameObject panel;

//    public TextMeshProUGUI scoreText;

//    void Awake()

//    {

//        Instance = this;

//        panel.SetActive(false);

//    }

//    public void Show(int score)
//    {
//        panel.SetActive(true);
//        scoreText.text = score.ToString();
//    }


//    public void RestartGame()

//    {

//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

//    }

//    public void Hide()

//    {

//        panel.SetActive(false);

//    }

//}


using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;
    public GameObject panel;
    [Header("Gameplay Score UI")]
    public TextMeshProUGUI gameplayScoreText;   // always visible

    [Header("Game Over UI")]
    public GameObject gameOverPanel;             // hidden until game over
    public TextMeshProUGUI gameOverScoreText;    // final score text

    private void Awake()
    {
        Instance = this;
        gameOverPanel.SetActive(false);
    }

    // 🔹 CALLED DURING GAMEPLAY
    public void UpdateScore(int score)
    {
        gameplayScoreText.text = score.ToString();
    }

    // 🔹 CALLED ON GAME OVER
    public void ShowGameOver(int score)
    {
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = score.ToString();
    }

    public void RestartGame()
    {
        Debug.Log("restart clicked");
        Time.timeScale = 1f; //  RESET FIRST
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
      public void Hide()

    {

        panel.SetActive(false);

    }

}


