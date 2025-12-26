using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float jumpTimeout = 0.3f;

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
        yield return new WaitForSeconds(jumpTimeout);


        if (hasJumped && !jumpResolved && isAlive)
        {
            DieImmediate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAlive)
            return;


        if (other.CompareTag("Border"))
        {
           
                DieImmediate();

            return;
        }
        

        if (!hasJumped || jumpResolved)
            return;


        if (other.CompareTag("Magnet"))
        {
            jumpResolved = true;
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

        if (!isAlive) return;

        if (jumpTimeoutRoutine != null)
            StopCoroutine(jumpTimeoutRoutine);

        hasJumped = false;
        jumpResolved = true;


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

}

