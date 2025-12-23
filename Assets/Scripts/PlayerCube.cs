using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour
{
    private float jumpForce = 100f;

    private Rigidbody rb;
    private bool hasJumped = false;
    private bool isAlive = true;

    [HideInInspector] public Transform targetWheel;
    [SerializeField] private Transform SpawnPos;
    private Coroutine jumpTimeoutRoutine;
    private bool landedThisJump = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            JumpToTarget();
        }
    }

    void JumpToTarget()
    {
        landedThisJump = false;
        hasJumped = true;
        GetComponent<Collider>().enabled = true;
        Debug.Log("player enter");
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
        yield return new WaitForSeconds(1.5f);

        if (hasJumped && isAlive)
        {
            Die();
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (!isAlive) return;
        if (!hasJumped) return;


        if (other.CompareTag("Cube"))
        {
            Die();
        }
    }

    public void AttachToMagnet(Transform wheel, Transform magnet)
    {
        if (jumpTimeoutRoutine != null)
            StopCoroutine(jumpTimeoutRoutine);

        landedThisJump = true;
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        transform.SetParent(null);

        transform.position = magnet.position;
        transform.rotation = magnet.rotation;
        transform.SetParent(wheel, true);

        hasJumped = false;

    }

    public void Die()
    {
        if (!isAlive) return;

        isAlive = false;
        rb.isKinematic = true;
        StartCoroutine(GameOverDelay());
    }

    IEnumerator GameOverDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        GameFlowController.Instance.GameOver();
    }
}