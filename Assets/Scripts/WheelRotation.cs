using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    public float rotationSpeed = 100f;

    void Update()
    {
        // Correct wheel spin (matches 2nd pic)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
