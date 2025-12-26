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

        scoreText.text = score.ToString();

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