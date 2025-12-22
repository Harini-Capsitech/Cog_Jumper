using UnityEngine;

public class WheelSpawner : MonoBehaviour
{
    public GameObject wheelPrefab;
    public float wheelSpacingZ = 15f;
    public float startZ = 0f;
    public float centerX = 0f;
    public float centerY = 0f;

    public GameObject SpawnWheelAtPoint(int index, Transform parent)
    {

        float zPos = startZ + (index * wheelSpacingZ);

        Vector3 spawnPos = new Vector3(centerX, centerY, zPos);

        Debug.Log($"Spawning wheel {index} at position: {spawnPos}");


        GameObject wheel = Instantiate(wheelPrefab, spawnPos, Quaternion.identity);
        wheel.name = $"Wheel_{index}";


        if (parent != null)
        {
            wheel.transform.SetParent(parent, true);
        }

        Debug.Log($"Wheel {index} final position: {wheel.transform.position}");

        return wheel;
    }
}

