//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerJump : MonoBehaviour
//{
//    public float jumpForce = 6f;

//    private Rigidbody rb;
//    private Transform targetMagnet;
//    private bool isJumping = false;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//    }

//    void Update()
//    {
//        if (Input.GetMouseButtonDown(0) && !isJumping)
//        {
//            Jump();
//        }
//    }

//    void Jump()
//    {
//        isJumping = true;

//        // Detach from wheel
//        transform.SetParent(null);

//        rb.isKinematic = false;
//        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
//    }

//    // Called by GameplayManager
//    public void SetNextMagnet(Transform magnet)
//    {
//        targetMagnet = magnet;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Magnet"))
//        {
//            // Stick to next wheel
//            rb.linearVelocity = Vector3.zero;
//            rb.isKinematic = true;

//            transform.position = other.transform.position;
//            transform.SetParent(other.transform);

//            isJumping = false;
//        }
//    }
//}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 6f;

    private Rigidbody rb;
    private Transform targetMagnet;
    private bool isJumping = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isJumping && IsJumpInputPressed())
        {
            WheelGameplayController.Instance.JumpToNextWheel(this);
            Jump();
        }
    }


    void Jump()
    {
        isJumping = true;

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void SetNextMagnet(Transform magnet)
    {
        targetMagnet = magnet;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Magnet"))
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;

            transform.position = other.transform.position;
            transform.SetParent(other.transform);

            isJumping = false;
        }
    }

    bool IsJumpInputPressed()
    {
        // Space key
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
            return true;

        // Mouse click
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }

}
