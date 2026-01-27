using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCube : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 150f;
   
    [Header("Game Over")]
    [SerializeField] private float gameOverDelay = 0.6f;

    private Rigidbody rb;
    public PlayerJumpEffect jumpEffect;

    public float shakeDuration = 0.2f;
    public float shakeStrength = 0.5f;
    public Camera cameraObj;

    [SerializeField]
    private Vector3 fixedAttachRotation = new Vector3(-85f, 12f, 192f);

    private bool hasAttachedOnce = false;
    private bool hasJumped = false;
    private bool jumpResolved = false;
    private bool isAlive = true;
    private bool inputLocked = false;
    private bool jumpSfxUnlocked = false;
    private bool gameOverStarted = false;
    
    [HideInInspector] public Transform targetWheel;
    [SerializeField] float steeringDuration = 0.25f;
    [SerializeField] float steeringStrength = 6f;
    [SerializeField] float upwardBias = 0.25f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        cameraObj = Camera.main;
    }

    void Update()
    {


        if (!isAlive) return;

        if (EventSystem.current.IsPointerOverGameObject()) return;
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
        dir.y += upwardBias;
        dir.Normalize();

        rb.linearVelocity = dir * jumpForce;

        StartCoroutine(SteerTowardTarget());
        StartCoroutine(CheckForGameOver());
    }

    IEnumerator SteerTowardTarget()
    {
        float timer = 0f;

        while (timer < steeringDuration)
        {
            timer += Time.fixedDeltaTime;

            Vector3 desiredDir =
                (targetWheel.position - rb.position).normalized;

            Vector3 desiredVelocity =
                desiredDir * rb.linearVelocity.magnitude;

            rb.linearVelocity = Vector3.Lerp(
                rb.linearVelocity,
                desiredVelocity,
                steeringStrength * Time.fixedDeltaTime
            );

            yield return new WaitForFixedUpdate();
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
        yield return new WaitForSecondsRealtime(0.5f);
        inputLocked = false;
    }

    //IEnumerator SmoothAttach(Transform wheel, Transform magnet)
    //{
    //    rb.isKinematic = true;
    //    GetComponent<Collider>().enabled = false;

    //    Vector3 startPos = transform.position;
    //    Quaternion startRot = transform.rotation;

    //    float t = 0f;
    //    float duration = 0.22f;

    //    while (t < 1f)
    //    {
    //        t += Time.deltaTime / duration;
    //        transform.position = Vector3.Lerp(startPos, magnet.position, t);
    //        transform.rotation = Quaternion.Slerp(startRot, magnet.rotation, t);
    //        yield return null;
    //    }

    //    transform.position = magnet.position;
    //    transform.rotation = magnet.rotation;
    //    transform.SetParent(wheel, true);
    //}


    //IEnumerator SmoothAttach(Transform wheel, Transform magnet)
    //{
    //    rb.isKinematic = true;
    //    GetComponent<Collider>().enabled = false;


    //    Vector3 outwardDir = (magnet.position - wheel.position).normalized;


    //    float surfaceOffset = 0.55f; // 🔧 tweak if needed

    //    Vector3 targetPos = magnet.position + outwardDir * surfaceOffset;

    //    //Quaternion targetRot = Quaternion.Euler(fixedAttachRotation);
    //    Quaternion targetRot = Quaternion.LookRotation(outwardDir, Vector3.up);

    //    Vector3 startPos = transform.position;
    //    Quaternion startRot = transform.rotation;

    //    float t = 0f;
    //    float duration = 0.22f;

    //    while (t < 1f)
    //    {
    //        t += Time.deltaTime / duration;
    //        transform.position = Vector3.Lerp(startPos, targetPos, t);
    //        transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
    //        yield return null;
    //    }

    //    transform.position = targetPos;
    //    transform.rotation = targetRot;

    //    // 4️⃣ Parent AFTER alignment
    //    transform.SetParent(wheel, true);
    //}

    IEnumerator SmoothAttach(Transform wheel, Transform magnet)
    {
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;

       
        transform.SetParent(wheel, false);

        
        transform.localPosition = new Vector3(0.0044f, -0.0017f, 0.0115f);
        transform.localRotation = Quaternion.Euler(-85f, 12f, 192f);
        transform.localScale = new Vector3(0.0018f, 0.0014f, 0.0018f);

        yield break;
    }



    public void ResetJumpState()
    {
        hasAttachedOnce = false;
        hasJumped = false;
        jumpResolved = false;
        inputLocked = false;
        jumpSfxUnlocked = false;
        gameOverStarted = false;

        isAlive = true;

        StopAllCoroutines();

        AppManager.instance.enableGameLogic();
    }
    void StartGameOver()
    {
        if (gameOverStarted) return;

        gameOverStarted = true;
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
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    
    public void DieImmediate()
    {
        if (!isAlive) return;
       
        if (Filler.IsPowerActive)
        {
            ForceAttachToTargetWheel();
            return;
        }
        
        GameFlowController.Instance.FinalGameOver();
        isAlive = false;
        inputLocked = true;

        rb.isKinematic = false;
        rb.useGravity = true;
        SoundManager.Instance.StopSfx();
        SoundManager.Instance.PlayGameOver();
    }

    IEnumerator CheckForGameOver()
    {
        Debug.Log("the game over check is running");
        yield return new WaitForSeconds(gameOverDelay);
        if (gameObject.transform.parent == null)
        {
         
            if (Filler.IsPowerActive)
            {
                ForceAttachToTargetWheel();
                yield break;
            }
           
            yield return new WaitForSeconds(1f);
            DieImmediate();
        }
    }
    void ForceAttachToTargetWheel()
    {
      
        if (jumpResolved) return;         
        
        if (targetWheel == null) return;

        GapTrigger gap = targetWheel.GetComponentInChildren<GapTrigger>(true);
        if (gap == null) return;

        Transform[] children = targetWheel.GetComponentsInChildren<Transform>();
        foreach (Transform t in children)
        {
            if (t.CompareTag("Magnet"))
            {
                AttachToMagnet(targetWheel, t);
                GameFlowController.Instance.PlayerLanded(gap);
                break;
            }
        }
    }

}


