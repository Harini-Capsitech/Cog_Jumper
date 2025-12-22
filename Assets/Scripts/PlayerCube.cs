using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour
{
    private float jumpForce = 90f;

    private Rigidbody rb;
    private bool hasJumped=false;
    private bool isAlive = true;

    [HideInInspector] public Transform targetWheel;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
       
        if (Input.GetMouseButtonDown(0) )
        {
            JumpToTarget();
        }
    }

    void JumpToTarget()
    {
        hasJumped = true;
        Debug.Log("player enter");
        GameFlowController.Instance.OnPlayerJumped();
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Vector3 dir = (targetWheel.position - transform.position).normalized;
        rb.AddForce(dir * jumpForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasJumped || !isAlive) return;
        if (other.GetComponent<GapTrigger>() != null)
            return;

        if (!other.CompareTag("Magnet"))
        {
            Die();
        }
    }

    public void AttachToMagnet(Transform wheel, Transform magnet)
    {
        rb.isKinematic = true;
        transform.SetParent(null);

        transform.position = magnet.position;
        transform.SetParent(wheel, true);

        hasJumped = false;

    }

    //public void AttachToMagnet(Transform wheel, Transform magnet)
    //{
    //    rb.isKinematic = true;

    //    // Parent first (local space)
    //    transform.SetParent(wheel, false);

    //    // Match cube exactly
    //    transform.localPosition = magnet.localPosition;
    //    transform.localRotation = magnet.localRotation;
    //    transform.localScale = magnet.localScale;

    //    hasJumped = false;
    //}


    public void Die()
    {
        if (!isAlive) return;

        isAlive = false;
        rb.isKinematic = true;
        StartCoroutine(GameOverDelay());
    }

    IEnumerator GameOverDelay()
    {
        yield return new WaitForSeconds(1.5f);
        GameFlowController.Instance.GameOver();
    }
   
}