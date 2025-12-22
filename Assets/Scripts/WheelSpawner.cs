using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WheelSpawner : MonoBehaviour
{
    public GameObject wheelPrefab;

    // Assign 3 spawn points from Inspector
    public Transform[] spawnPoints;

    public float rotationSpeed = 120f;

    // Game Over UI
    public GameObject gameOverPanel;
    void Start()
    {
        gameOverPanel.SetActive(false);

        SpawnWheels();

        // Trigger Game Over after 5 seconds
        StartCoroutine(GameOverAfterDelay());
    }

    void SpawnWheels()
    {
        if (spawnPoints == null || spawnPoints.Length < 3)
        {
            Debug.LogError("Please assign 3 spawn points!");
            return;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject wheel = Instantiate(
                wheelPrefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            WheelRotation rotation = wheel.AddComponent<WheelRotation>();

            // optional: alternate rotation direction
            rotation.rotationSpeed = (i % 2 == 0)
                ? rotationSpeed
                : -rotationSpeed;
        }
    }
    IEnumerator GameOverAfterDelay()
    {
        yield return new WaitForSeconds(10f);
        GameOver();
    }

    void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // pause game
    }
    //  Called from Restart Button (OnClick)
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
