using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    public GameObject panel;
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false); 
    }

    public void Show(int score)
    {
        panel.SetActive(true);
        scoreText.text = "Score: " + score;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
    public void Hide()
    {
        panel.SetActive(false);
    }
}
