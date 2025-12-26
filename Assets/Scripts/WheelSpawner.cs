using UnityEngine;

public class WheelSpawner : MonoBehaviour
{
    public GameObject wheelPrefab;

    [Header("Forward Movement")]
    public float wheelSpacingZ = 35f;
    public float startZ = 3f;
    public float centerY = 0f;

    public float centerX = 0f;
    public float leftOffset = 1.5f;   
    public float rightOffset = 10f; 

    public GameObject SpawnWheel(int index, Transform parent)
    {
        float zPos = startZ + index * wheelSpacingZ;

        float xPos;
        if (index % 2 == 0)
        {
            
            xPos = centerX - leftOffset;
        }
        else
        {
            
            xPos = centerX + rightOffset;
        }

        Vector3 spawnPos = new Vector3(xPos, centerY, zPos);

        GameObject wheel = Instantiate(wheelPrefab, spawnPos, Quaternion.identity);
        wheel.name = $"Wheel_{index}";

        if (parent != null)
            wheel.transform.SetParent(parent, true);

        return wheel;
    }
}
