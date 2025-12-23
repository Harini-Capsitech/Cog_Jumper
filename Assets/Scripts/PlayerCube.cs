//using UnityEngine;
//using System.Collections;

//public class PlayerCube : MonoBehaviour
//{
//    private float jumpForce = 250f;
//    [SerializeField] private float allowedJumpAngle = 30f; // degrees (30–45 feels good)

//    private Rigidbody rb;
//    private bool hasJumped = false;
//    private bool isAlive = true;
//    public bool isJumped = false;

//    [HideInInspector] public Transform targetWheel;
//    [SerializeField] private Transform SpawnPos;
//    private Coroutine jumpTimeoutRoutine;
//    private bool landedThisJump = false;


//    void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        rb.isKinematic = true;
//    }

//    void Update()
//    {

//        //if (Input.GetMouseButtonDown(0))
//        //{
//        //    JumpToTarget();

//        //}
//        if (Input.GetMouseButtonDown(0) && isAlive && !hasJumped)
//        {
//            if (IsJumpAngleValid())
//            {
//                JumpToTarget();
//            }
//            else
//            {

//                Die();
//            }
//        }

//    }

//    void JumpToTarget()
//    {
//        landedThisJump = false;
//        hasJumped = true;
//        isJumped = true;
//        GetComponent<Collider>().enabled = true;
//        Debug.Log("player enter");
//        GameFlowController.Instance.OnPlayerJumped();
//        transform.SetParent(null);
//        rb.isKinematic = false;
//        rb.linearVelocity = Vector3.zero;
//        rb.angularVelocity = Vector3.zero;
//        Vector3 dir = (targetWheel.position - transform.position).normalized;
//        rb.AddForce(dir * jumpForce, ForceMode.Impulse);
//        jumpTimeoutRoutine = StartCoroutine(JumpTimeout());
//    }
//    IEnumerator JumpTimeout()
//    {
//        yield return new WaitForSeconds(0.3f);

//        if (hasJumped && !landedThisJump && isAlive)
//        {
//            Die();
//        }
//    }



//    private void OnTriggerEnter(Collider other)
//    {
//        if (!isAlive || !hasJumped) return;



//        if (other.gameObject.layer == LayerMask.NameToLayer("Magnet"))
//            return;



//    }

//    public void AttachToMagnet(Transform wheel, Transform magnet)
//    {
//        if (jumpTimeoutRoutine != null)
//            StopCoroutine(jumpTimeoutRoutine);

//        landedThisJump = true;
//        hasJumped = false;
//        rb.isKinematic = true;
//        GetComponent<Collider>().enabled = false;
//        transform.SetParent(null);

//        transform.position = magnet.position;
//        transform.rotation = magnet.rotation;
//        transform.SetParent(wheel, true);



//    }

//    bool IsJumpAngleValid()
//    {
//        if (transform.parent == null) return false;

//        Transform wheel = transform.parent;
//        Camera cam = Camera.main;

//        // Convert positions into CAMERA SPACE
//        Vector3 wheelCamPos = cam.transform.InverseTransformPoint(wheel.position);
//        Vector3 playerCamPos = cam.transform.InverseTransformPoint(transform.position);

//        // Direction from wheel center to player IN CAMERA SPACE
//        Vector3 dirCam = (playerCamPos - wheelCamPos).normalized;

//        // Camera-space UP direction
//        Vector3 camUp = Vector3.up;

//        // Angle between player direction and screen-up
//        float angle = Vector3.Angle(dirCam, camUp);

//        Debug.Log($"Camera-space angle: {angle}");

//        // Allow jump ONLY when cube is near top of screen
//        return angle <= allowedJumpAngle;
//    }




//    public void Die()
//    {
//        if (!isAlive) return;

//        isAlive = false;
//        rb.isKinematic = true;
//        StartCoroutine(GameOverDelay());
//    }

//    IEnumerator GameOverDelay()
//    {
//        yield return new WaitForSecondsRealtime(0.3f);
//        GameFlowController.Instance.GameOver();
//    }

//}

using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 250f;
    [SerializeField] private float jumpTimeout = 0.35f;

    private Rigidbody rb;

    private bool hasJumped = false;
    private bool jumpResolved = false;
    private bool isAlive = true;

    [HideInInspector] public Transform targetWheel;

    private Coroutine jumpTimeoutRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        if (!isAlive) return;

        if (Input.GetMouseButtonDown(0) && !hasJumped)
        {
            JumpToTarget();
        }
    }

    void JumpToTarget()
    {
        hasJumped = true;
        jumpResolved = false;

        GetComponent<Collider>().enabled = true;

        GameFlowController.Instance.OnPlayerJumped();

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 dir = (targetWheel.position - transform.position).normalized;
        rb.AddForce(dir * jumpForce, ForceMode.Impulse);

        jumpTimeoutRoutine = StartCoroutine(JumpTimeout());
    }

    IEnumerator JumpTimeout()
    {
        yield return new WaitForSeconds(jumpTimeout);

        // ❌ Failed to touch Magnet in time
        if (hasJumped && !jumpResolved && isAlive)
        {
            DieDelayed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAlive || !hasJumped || jumpResolved)
            return;

        // ✅ SUCCESS: Magnet touched first
        if (other.CompareTag("Magnet"))
        {
            jumpResolved = true;
            return;
        }

        // ❌ FAILURE: FixedCube touched → IMMEDIATE GAME OVER
        if (other.CompareTag("FixedCube"))
        {
            jumpResolved = true;

            if (jumpTimeoutRoutine != null)
                StopCoroutine(jumpTimeoutRoutine);

            GetComponent<Collider>().enabled = false;

            DieImmediate();
        }
    }

    public void AttachToMagnet(Transform wheel, Transform magnet)
    {
        // 🔒 Block landing after death
        if (!isAlive) return;

        if (jumpTimeoutRoutine != null)
            StopCoroutine(jumpTimeoutRoutine);

        hasJumped = false;
        jumpResolved = true;

        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;

        transform.position = magnet.position;
        transform.rotation = magnet.rotation;
        transform.SetParent(wheel, true);
    }

    // ❌ Immediate Game Over (FixedCube hit)
    void DieImmediate()
    {
        if (!isAlive) return;

        isAlive = false;
        rb.isKinematic = true;

        GameFlowController.Instance.GameOver();
    }

    // ❌ Delayed Game Over (timeout miss)
    void DieDelayed()
    {
        if (!isAlive) return;

        isAlive = false;
        rb.isKinematic = true;

        StartCoroutine(GameOverDelay());
    }

    IEnumerator GameOverDelay()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        GameFlowController.Instance.GameOver();
    }
}
