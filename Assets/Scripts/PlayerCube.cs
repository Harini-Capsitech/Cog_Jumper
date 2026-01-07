using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float jumpTimeout = 0.5f;

    [Header("Game Over")]
    [SerializeField] private float gameOverDelay = 0.1f;

    private Rigidbody rb;
    public PlayerJumpEffect jumpEffect;

    private bool hasAttachedOnce = false;
    private bool hasJumped = false;
    private bool jumpResolved = false;
    private bool isAlive = true;
    private bool inputLocked = false;
    private bool jumpSfxUnlocked = false;
    private bool gameOverStarted = false;

    [HideInInspector] public Transform targetWheel;
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
        GameFlowController.Instance.OnPlayerJumped();

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 dir = (targetWheel.position - transform.position).normalized;
        dir.y += 0.25f;
        rb.linearVelocity = dir * jumpForce;
        Invoke(nameof(JumpTimeout), 0.5f); 
        
    }

    void JumpTimeout()
    {
        
        if (gameObject.transform.parent == null)
        {
            StartGameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAlive) return;

        if (other.CompareTag("Border"))
        {
            StartGameOver();
            return;
        }

        if (!hasJumped || jumpResolved) return;

        if (other.CompareTag("FixedCube"))
        {
            jumpResolved = true;
            StartGameOver();
            return;
        }

        if (other.CompareTag("Magnet"))
        {
            jumpResolved = true;
            StartCoroutine(InputLockCoroutine());
        }
    }

    public void AttachToMagnet(Transform wheel, Transform magnet)
    {
        

        if (!isAlive) return;

        CancelGameOver();

        hasJumped = false;
        jumpResolved = true;
        StartCoroutine(InputLockCoroutine());
        if (!jumpSfxUnlocked)
            jumpSfxUnlocked = true;
        else
            SoundManager.Instance.PlayJump();

        if (hasAttachedOnce)
            jumpEffect?.PlayAttachEffect(0.1f);
        else
            hasAttachedOnce = true;

        StartCoroutine(SmoothAttach(wheel, magnet));
    }
    IEnumerator InputLockCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        inputLocked = false;
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

    void StartGameOver()
    {
        if (gameOverStarted) return;

        gameOverStarted = true;

        
        GameFlowController.Instance.PreGameOverCleanup();

        StartCoroutine(DelayedGameOver());
    }

    void CancelGameOver()
    {
        gameOverStarted = false;
        StopAllCoroutines();
    }

    public void ResetPlayerState()
    {
        StopAllCoroutines();

        hasAttachedOnce = false;
        hasJumped = false;
        jumpResolved = false;
        isAlive = true;
        inputLocked = false;
        jumpSfxUnlocked = false;
        gameOverStarted = false;

        targetWheel = null;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    IEnumerator DelayedGameOver()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);
        DieImmediate();
        yield return null;
    }

    void DieImmediate()
    {
        if (!isAlive) return;


        GameFlowController.Instance.FinalGameOver();
        isAlive = false;
        inputLocked = true;

        rb.isKinematic = false;
        rb.useGravity = true;
        SoundManager.Instance.StopSfx();
        SoundManager.Instance.PlayGameOver();
    }
}
