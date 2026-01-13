using UnityEngine;

public class RodRotator : MonoBehaviour
{
    public float speed = 60f;

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
