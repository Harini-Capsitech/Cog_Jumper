//using UnityEngine;
//using System.Collections;

//public class PlayerCube : MonoBehaviour
//{
//    [Header("Jump Settings")]
//    [SerializeField] private float jumpForce = 100f;
//    [SerializeField] private float jumpTimeout = 0.1f;

//    private Rigidbody rb;
//    public PlayerJumpEffect jumpEffect; // assign in Inspector
//    private bool hasAttachedOnce = false;
//    private bool hasJumped = false;
//    private bool jumpResolved = false;
//    private bool isAlive = true;
//    private bool inputLocked = false;

//    [HideInInspector] public Transform targetWheel;

//    private Coroutine jumpTimeoutRoutine;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        rb.isKinematic = true;
//    }



//    void Update()
//    {
//        if (!isAlive) return;

//        if (Input.GetMouseButtonDown(0) && !hasJumped)
//        {
//            inputLocked = true;
//            Debug.Log("Clicked multiple times");
//            JumpToTarget();
//        }

//    }

//    void JumpToTarget()
//    {
//        hasJumped = true;
//        jumpResolved = false;

//        GetComponent<Collider>().enabled = true;
//        SoundManager.Instance.PlayJump();

//        GameFlowController.Instance.OnPlayerJumped();
//        // Camera.main.GetComponent<CameraFollow>().SetTarget(this.transform);

//        transform.SetParent(null);

//        rb.isKinematic = false;
//        rb.linearVelocity = Vector3.zero;
//        rb.angularVelocity = Vector3.zero;

//        Vector3 dir = (targetWheel.position - transform.position).normalized;
//        dir.y += 0.25f;
//        rb.linearVelocity = dir * jumpForce;

//        jumpTimeoutRoutine = StartCoroutine(JumpTimeout());
//    }

//    IEnumerator JumpTimeout()
//    {
//        yield return new WaitForSeconds(jumpTimeout);


//        if (hasJumped && !jumpResolved && isAlive)
//        {
//            Debug.Log("Enter");
//            //Camera.main.GetComponent<CameraFollow>().SetTarget(this.transform);
//            StartCoroutine(GameOverDelay());

//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!isAlive)
//            return;


//        if (other.CompareTag("Border"))
//        {
//            Debug.Log("Border gameover");

//            StartCoroutine(GameOverDelay());
//            return;
//        }


//        if (!hasJumped || jumpResolved)
//            return;


//        if (other.CompareTag("Magnet"))
//        {
//            jumpResolved = true;
//            return;
//        }


//        if (other.CompareTag("FixedCube"))
//        {
//            jumpResolved = true;
//            DieImmediate();
//        }
//    }


//    public void AttachToMagnet(Transform wheel, Transform magnet)
//    {
//        SoundManager.Instance.PlayJump();

//        if (!isAlive) return;
//        // Camera.main.GetComponent<CameraFollow>().SetTarget(null);
//        if (jumpTimeoutRoutine != null)
//            StopCoroutine(jumpTimeoutRoutine);

//        hasJumped = false;
//        jumpResolved = true;
//        inputLocked = false;
//        // Play effect ONLY after first successful jump
//        if (hasAttachedOnce)
//        {
//            if (jumpEffect != null)
//                jumpEffect.PlayAttachEffect(0.1f);
//        }
//        else
//        {
//            hasAttachedOnce = true; // skip first attach
//        }

//        StartCoroutine(SmoothAttach(wheel, magnet));
//    }

//    IEnumerator SmoothAttach(Transform wheel, Transform magnet)
//    {
//        rb.isKinematic = true;
//        GetComponent<Collider>().enabled = false;

//        Vector3 startPos = transform.position;
//        Quaternion startRot = transform.rotation;

//        float t = 0f;
//        float duration = 0.22f;

//        while (t < 1f)
//        {
//            t += Time.deltaTime / duration;
//            transform.position = Vector3.Lerp(startPos, magnet.position, t);
//            transform.rotation = Quaternion.Slerp(startRot, magnet.rotation, t);
//            yield return null;
//        }

//        transform.position = magnet.position;
//        transform.rotation = magnet.rotation;
//        transform.SetParent(wheel, true);
//    }

//    void DieImmediate()
//    {
//        if (!isAlive) return;

//        isAlive = false;
//        //inputLocked = true;
//        rb.isKinematic = true;


//        SoundManager.Instance.StopSfx();

//        SoundManager.Instance.PlayGameOver();

//        GameFlowController.Instance.GameOver();
//    }

//    IEnumerator GameOverDelay()
//    {
//        yield return new WaitForSeconds(0.1f);
//        DieImmediate();
//    }



//}

using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float jumpTimeout = 0.1f;

    private Rigidbody rb;
    public PlayerJumpEffect jumpEffect;

    private bool hasAttachedOnce = false;
    private bool hasJumped = false;
    private bool jumpResolved = false;
    private bool landedThisJump = false;   // ✅ NEW (critical)
    private bool isAlive = true;
    private bool inputLocked = false;

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

        // ✅ Single tap only
        if (Input.GetMouseButtonDown(0) && !hasJumped && !inputLocked)
        {
            inputLocked = true;
            JumpToTarget();
        }
    }

    void JumpToTarget()
    {
        hasJumped = true;
        jumpResolved = false;
        landedThisJump = false;

        GetComponent<Collider>().enabled = true;
        SoundManager.Instance.PlayJump();

        GameFlowController.Instance.OnPlayerJumped();

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 dir = (targetWheel.position - transform.position).normalized;
        dir.y += 0.25f;
        rb.linearVelocity = dir * jumpForce;

        jumpTimeoutRoutine = StartCoroutine(JumpTimeout());
    }

    IEnumerator JumpTimeout()
    {
        // ✅ MUST ignore Time.timeScale
        yield return new WaitForSecondsRealtime(jumpTimeout);

        // ❌ Missed completely → GameOver
        if (hasJumped && !landedThisJump && isAlive)
        {
            StartCoroutine(GameOverDelay());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAlive)
            return;

        if (!hasJumped || jumpResolved)
            return;

        // ✅ Touching magnet = potential success (NOT final)
        if (other.CompareTag("Magnet"))
        {
            landedThisJump = true;
            return;
        }

        // ❌ Immediate failure
        if (other.CompareTag("FixedCube"))
        {
            jumpResolved = true;
            DieImmediate();
        }
    }

    public void AttachToMagnet(Transform wheel, Transform magnet)
    {
        if (!isAlive) return;

        SoundManager.Instance.PlayJump();

        if (jumpTimeoutRoutine != null)
            StopCoroutine(jumpTimeoutRoutine);

        hasJumped = false;
        jumpResolved = true;
        landedThisJump = false;
        inputLocked = false;

        // Skip effect on very first attach
        if (hasAttachedOnce)
        {
            if (jumpEffect != null)
                jumpEffect.PlayAttachEffect(0.1f);
        }
        else
        {
            hasAttachedOnce = true;
        }

        StartCoroutine(SmoothAttach(wheel, magnet));
    }

    IEnumerator SmoothAttach(Transform wheel, Transform magnet)
    {
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0f;
        float duration = 0.22f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPos, magnet.position, t);
            transform.rotation = Quaternion.Slerp(startRot, magnet.rotation, t);
            yield return null;
        }

        transform.position = magnet.position;
        transform.rotation = magnet.rotation;
        transform.SetParent(wheel, true);
    }

    void DieImmediate()
    {
        if (!isAlive) return;

        isAlive = false;
        rb.isKinematic = true;

        SoundManager.Instance.StopSfx();
        SoundManager.Instance.PlayGameOver();

        GameFlowController.Instance.GameOver();
    }

    IEnumerator GameOverDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        DieImmediate();
    }
}

