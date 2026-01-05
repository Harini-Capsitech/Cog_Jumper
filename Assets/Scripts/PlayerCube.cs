using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float jumpTimeout = 0.5f;

    private Rigidbody rb;
    public PlayerJumpEffect jumpEffect; 
    private bool hasAttachedOnce = false;
    private bool hasJumped = false;
    private bool jumpResolved = false;
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

        if (Input.GetMouseButtonDown(0) && !inputLocked)
        {
            inputLocked = true;
            JumpToTarget();
        }

    }

    void JumpToTarget()
    {
        hasJumped = true;
        jumpResolved = false;

        GetComponent<Collider>().enabled = true;
        SoundManager.Instance.PlayJump();

        GameFlowController.Instance.OnPlayerJumped();
        // Camera.main.GetComponent<CameraFollow>().SetTarget(this.transform);
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
        yield return new WaitForSeconds(jumpTimeout);


        if (hasJumped && !jumpResolved && isAlive)
        {
            Debug.Log("Enter");
            DieImmediate();
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAlive)
            return;


        if (other.CompareTag("Border"))
        {
            Debug.Log("Border gameover");
            //  Camera.main.GetComponent<CameraFollow>().SetTarget(this.transform);
            DieImmediate();
            return;
        }


        if (!hasJumped || jumpResolved)
            return;


        if (other.CompareTag("Magnet"))
        {
            jumpResolved = true;
            inputLocked = false;
            return;
        }


        if (other.CompareTag("FixedCube"))
        {
            jumpResolved = true;
            DieImmediate();
        }
    }

    
    public void AttachToMagnet(Transform wheel, Transform magnet)
    {
        SoundManager.Instance.PlayJump();

        if (!isAlive) return;
        
        if (jumpTimeoutRoutine != null)
            StopCoroutine(jumpTimeoutRoutine);

        hasJumped = false;
        jumpResolved = true;
        inputLocked = false;

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
        inputLocked = true;
        rb.isKinematic = true;

        //// ✅ CAMERA FOLLOW ONLY ON GAME OVER
        //Camera.main.GetComponent<GameOverCameraFollow>()
        //           .StartFollow(transform);  

        SoundManager.Instance.StopSfx();

        SoundManager.Instance.PlayGameOver();

        GameFlowController.Instance.GameOver();
    }


}
