using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    public float rotationSpeed = 250f;

    void Update()
    {
        transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}

