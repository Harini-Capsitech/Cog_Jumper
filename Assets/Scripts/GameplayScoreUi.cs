using UnityEngine;
using TMPro;

public class GameplayScoreUI : MonoBehaviour
{
    public static GameplayScoreUI Instance;

    [SerializeField] private TextMeshProUGUI scoreText;

    void Awake()
    {
        Instance = this;
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
}