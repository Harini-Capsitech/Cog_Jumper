using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
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

        //PLAY JUMP SOUND
        //SoundManager.Instance.PlayJump();
        //

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


        if (hasJumped && !jumpResolved && isAlive)
        {
            DieDelayed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAlive || !hasJumped || jumpResolved)
            return;


        if (other.CompareTag("Magnet"))
        {
            jumpResolved = true;
            return;
        }


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


    void DieImmediate()
    {
        if (!isAlive) return;

        isAlive = false;
        rb.isKinematic = true;

        // to stop sound after GAME starts .
        //SoundManager.Instance.StopSfx();

        //Gameover sond
       // SoundManager.Instance.PlayGameOver();
        //
        GameFlowController.Instance.GameOver();
    }


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
