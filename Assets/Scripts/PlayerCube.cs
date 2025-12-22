using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour
{
    public float jumpForce = 40f;

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

        if (!other.CompareTag("Magnet"))
        {
            Die();
        }
    }

    public void AttachToMagnet(Transform wheel, Transform magnet)
    {
        Debug.Log($"AttachToMagnet - Wheel: {wheel.name}, Magnet: {magnet.name}");
        Debug.Log($"Magnet world pos: {magnet.position}, local pos: {magnet.localPosition}");

        
        rb.isKinematic = true;
        transform.SetParent(null);

        
        transform.position = magnet.position;

        Debug.Log($"Player moved to: {transform.position}");

        
        transform.SetParent(wheel, true);

        hasJumped = false;

        Debug.Log($"Player parented to {wheel.name}, final pos: {transform.position}");
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
        yield return new WaitForSeconds(1.5f);
        GameFlowController.Instance.GameOver();
    }
}