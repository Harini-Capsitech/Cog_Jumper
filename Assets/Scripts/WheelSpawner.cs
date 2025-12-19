//using UnityEngine;
//using System.Collections;

//public class WheelSpawner : MonoBehaviour
//{
//    public GameObject wheelPrefab;

//    // Borders
//    public Transform leftBorder;    // B
//    public Transform rightBorder;   // B2

//    public float rotationSpeed = 120f;
//    public float spawnDelay = 2f;

//    // Offsets
//    public float verticalSpacing = 2.5f;
//    public float horizontalInset = 1.2f;

//    private int spawnIndex = 0;
//    private bool isGameOver = false;

//    void Start()
//    {
//        StartCoroutine(SpawnLoop());
//    }

//    IEnumerator SpawnLoop()
//    {
//        while (!isGameOver)
//        {
//            SpawnSingleWheel();
//            yield return new WaitForSeconds(spawnDelay);
//        }
//    }

//    void SpawnSingleWheel()
//    {
//        Vector3 spawnPosition = GetNextSpawnPosition();

//        GameObject wheel = Instantiate(
//            wheelPrefab,
//            spawnPosition,
//            Quaternion.identity
//        );

//        WheelRotation rotation = wheel.AddComponent<WheelRotation>();

//        // rotation logic unchanged
//        rotation.rotationSpeed = (spawnIndex % 2 == 0)
//            ? rotationSpeed
//            : -rotationSpeed;

//        spawnIndex++;
//    }

//    Vector3 GetNextSpawnPosition()
//    {
//        int pattern = spawnIndex % 4;
//        int row = spawnIndex / 2; // vertical stacking

//        switch (pattern)
//        {
//            case 0: // Bottom Right
//                return new Vector3(
//                    rightBorder.position.x - horizontalInset,
//                    -row * verticalSpacing,
//                    0f
//                );

//            case 1: // Top Left
//                return new Vector3(
//                    leftBorder.position.x + horizontalInset,
//                    -row * verticalSpacing,
//                    0f
//                );

//            case 2: // Top Right
//                return new Vector3(
//                    rightBorder.position.x - horizontalInset,
//                    -row * verticalSpacing,
//                    0f
//                );

//            case 3: // Top Left again
//                return new Vector3(
//                    leftBorder.position.x + horizontalInset,
//                    -row * verticalSpacing,
//                    0f
//                );
//        }

//        return Vector3.zero;
//    }

//    public void GameOver()
//    {
//        isGameOver = true;
//    }
//}
using UnityEngine;

public class WheelSpawner : MonoBehaviour
{
    public GameObject wheelPrefab;

    // Assign 3 spawn points from Inspector
    public Transform[] spawnPoints;

    public float rotationSpeed = 120f;

    void Start()
    {
        SpawnWheels();
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
}
