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
        Debug.Log("RESTART CLICKED");  //add
       // Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
    public void Hide()
    {
        panel.SetActive(false);
    }
}