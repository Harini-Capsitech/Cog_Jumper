using UnityEngine;
using System.Collections;

public class WheelSpawner : MonoBehaviour
{
    public GameObject wheelPrefab;

    //Borders
    public Transform leftBorder;    // B
    public Transform rightBorder;   // B2

    public float rotationSpeed = 120f;

    // Offsets inside borders
    public Vector3 topLeftOffset = new Vector3(1.5f, 3.0f, 0f);
    public Vector3 topRightOffset = new Vector3(-1.5f, 3.0f, 0f);
    public Vector3 bottomRightOffset = new Vector3(-1.5f, -3.5f, 0f);

    //  Spawn control
    public float spawnDelay = 2f;

    private int spawnIndex = 0;
    private bool isGameOver = false;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (!isGameOver)
        {
            SpawnTwoWheels();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnTwoWheels()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnPosition = GetNextSpawnPosition();

            GameObject wheel = Instantiate(
                wheelPrefab,
                spawnPosition,
                Quaternion.identity
            );

            WheelRotation rotation = wheel.AddComponent<WheelRotation>();

            //  rotation logic UNCHANGED
            rotation.rotationSpeed = (spawnIndex % 2 == 0)
                ? rotationSpeed
                : -rotationSpeed;

            spawnIndex++;
        }
    }

    Vector3 GetNextSpawnPosition()
    {
        int pattern = spawnIndex % 4;

        switch (pattern)
        {
            case 0: // Bottom Right
                return rightBorder.position + bottomRightOffset;

            case 1: // Top Left
                return leftBorder.position + topLeftOffset;

            case 2: // Top Right
                return rightBorder.position + topRightOffset;

            case 3: // Top Left again
                return leftBorder.position + topLeftOffset;
        }

        // fallback (never really used)
        return Vector3.zero;
    }

    //  Call this when game ends
    public void GameOver()
    {
        isGameOver = true;
    }
}
