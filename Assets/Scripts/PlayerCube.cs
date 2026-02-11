using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCube : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 150f;
    [SerializeField] private float allowedTapAngle = 60f;
    [SerializeField] private float upwardBias = 0.25f;

    [Header("Game Over")]
    [SerializeField] private float gameOverDelay = 0.25f;
    public PlayerJumpEffect jumpEffect;
    private bool hasAttachedOnce = false;
    private Rigidbody rb;
    private Vector3 originalScale;

    [HideInInspector] public Transform targetWheel;
    [HideInInspector] public Transform currentWheel;

    private Transform selectedMagnet;

    private bool isAlive = true;
    private bool isAttached = true;
    private bool isJumping = false;
    private bool inputLocked = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        originalScale = new Vector3(3f,3f,3f);

        Debug.Log("[PlayerCube] Awake completed");
    }

    void Update()
    {
        if (!isAlive)
        {
            Debug.Log("[PlayerCube] Update blocked → isAlive = false");
            return;
        }

        if (!isAttached)
            return;

        if (inputLocked)
        {
            Debug.Log("[PlayerCube] Input locked");
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("[PlayerCube] Pointer over UI → Ignored");
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("[PlayerCube] Tap detected");
            inputLocked = true;
            TryJump();
        }
    }

    
    void TryJump()
    {
        Debug.Log("[PlayerCube] TryJump called");

        if (!IsTapAngleValid(out selectedMagnet))
        {
            Debug.Log("[PlayerCube] ❌ Tap angle invalid → GameOverRoutine started");
            StartCoroutine(GameOverRoutine());
            return;
        }

        Debug.Log($"[PlayerCube] ✅ Tap valid → Selected Magnet: {selectedMagnet.name}");
        StartJump();
    }

    bool IsTapAngleValid(out Transform nearestMagnet)
    {
        nearestMagnet = null;

        if (targetWheel == null)
        {
            Debug.Log("[PlayerCube] ❌ targetWheel is NULL");
            return false;
        }

        Debug.Log($"[PlayerCube] Checking magnets on targetWheel: {targetWheel.name}");

        Vector3 origin = transform.position;
        Vector3 toWheel = (targetWheel.position - origin).normalized;

        float bestDistance = float.MaxValue;

        GapTrigger[] gaps = targetWheel.GetComponentsInChildren<GapTrigger>(true);

        Debug.Log($"[PlayerCube] Found {gaps.Length} GapTriggers on targetWheel");

        foreach (var gap in gaps)
        {
            if (gap.snapMagnet == null)
            {
                Debug.Log($"[PlayerCube] Gap {gap.name} has NULL snapMagnet");
                continue;
            }

            Vector3 toMagnet = gap.snapMagnet.position - origin;
            float angle = Vector3.Angle(toWheel, toMagnet.normalized);

            Debug.Log($"[PlayerCube] Checking magnet {gap.snapMagnet.name} → Angle = {angle}");

            if (angle > allowedTapAngle)
            {
                Debug.Log($"[PlayerCube] ❌ Angle too large (> {allowedTapAngle})");
                continue;
            }

            float dist = toMagnet.magnitude;

            if (dist < bestDistance)
            {
                bestDistance = dist;
                nearestMagnet = gap.snapMagnet;
            }
        }

        if (nearestMagnet == null)
            Debug.Log("[PlayerCube] ❌ No valid magnet found");

        return nearestMagnet != null;
    }

    void StartJump()
    {
        Debug.Log("[PlayerCube] 🚀 StartJump");

        isAttached = false;
        isJumping = true;

        Debug.Log($"[PlayerCube] CurrentWheel before jump: {(currentWheel != null ? currentWheel.name : "NULL")}");

        GameFlowController.Instance.OnPlayerJumped();

        transform.SetParent(null);
        transform.localScale = originalScale;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 dir = (selectedMagnet.position - transform.position).normalized;
        dir.y += upwardBias;

        Debug.Log($"[PlayerCube] Jump direction: {dir}");

        rb.linearVelocity = dir * jumpForce;

        Debug.Log($"[PlayerCube] Applied velocity: {rb.linearVelocity}");

        StartCoroutine(GameOverRoutine());
    }

  
    public void AttachToMagnet(Transform wheel, Transform magnet)
    {
        if (!isAlive || magnet == null)
        {
            Debug.Log("[PlayerCube] ❌ AttachToMagnet aborted → isAlive false or magnet null");
            return;
        }

        Debug.Log($"[PlayerCube] ✅ Attaching to wheel: {wheel.name} → magnet: {magnet.name}");

        StopAllCoroutines();

        currentWheel = wheel;

        isAttached = true;
        isJumping = false;
        inputLocked = false;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(magnet, false);
        transform.localPosition = new Vector3(-1.8f,0.4f,-1f);
        transform.localRotation = Quaternion.identity;
        transform.Rotate(-87f, 113f, -40f);
        transform.localScale = new Vector3(1.5f,1.5f,1.5f);
        // 🔥 PLAY ATTACH EFFECT
        if (hasAttachedOnce)
        {
            jumpEffect?.PlayAttachEffect(0.1f);
            Debug.Log("[PlayerCube] ✨ Jump Attach Effect Played");
        }
        else
        {
            hasAttachedOnce = true;
            Debug.Log("[PlayerCube] First attachment → Effect skipped");
        }
    }

    IEnumerator GameOverRoutine()
    {
        Debug.Log("[PlayerCube] GameOverRoutine started");

        yield return new WaitForSeconds(gameOverDelay);

        if (!isAttached && isJumping)
        {
            Debug.Log("[PlayerCube] ❌ Player did not attach → dying in 0.8s");

            yield return new WaitForSeconds(0.8f);
            DieImmediate();
        }
        else
        {
            Debug.Log("[PlayerCube] Player attached successfully before timeout");
        }
    }

    public void DieImmediate()
    {
        if (!isAlive)
            return;

        Debug.Log("[PlayerCube] 💀 DieImmediate called");

        isAlive = false;
        inputLocked = true;

        rb.isKinematic = false;
        rb.useGravity = true;

        GameFlowController.Instance.FinalGameOver();
    }

    
    public void ResetPlayerState()
    {
        Debug.Log("[PlayerCube] ResetPlayerState called");

        StopAllCoroutines();

        isAlive = true;
        isAttached = true;
        isJumping = false;
        inputLocked = false;

        targetWheel = null;
        currentWheel = null;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(null);
        transform.localScale = originalScale;
    }

    public void ResetJumpState()
    {
        Debug.Log("[PlayerCube] ResetJumpState called");
        ResetPlayerState();
    }

    public bool IsJumping => isJumping;
}
  
