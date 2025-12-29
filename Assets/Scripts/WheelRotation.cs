using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    [Tooltip("1 = Clockwise, -1 = Anti-Clockwise")]
    public int rotationDirection = -1;

    void Start()
    {
        
        rotationDirection = Random.value > 0.5f ? 1 : -1;
    }

    void Update()
    {
        float speed = GameFlowController.CurrentWheelSpeed;

        transform.Rotate(
            Vector3.up * speed * rotationDirection * Time.deltaTime,
            Space.Self
        );
    }
}

