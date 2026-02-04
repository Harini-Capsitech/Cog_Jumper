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
    private int count = 0;

    [HideInInspector] public Transform targetWheel;
    [SerializeField] float steeringDuration = 0.25f;
    [SerializeField] float steeringStrength = 6f;
    [SerializeField] float upwardBias = 0.25f;
    [SerializeField] private Transform head;
    [SerializeField] private Transform head1;
    [SerializeField] private Transform head2;
    private RaycastHit groundHit;
             
    [SerializeField] LayerMask targetMask;
    public Transform HitWheel;
    private Vector3 rayHitPoint;
    private Vector3 rayHitNormal;
    private Transform rayHitWheel;

    [SerializeField] float castRadius = 0.8f;  
    [SerializeField] float castDistance = 60f;
    private static bool tutorialHintConsumed = false;

    RaycastHit hit;
    bool hasValidTarget;
    bool hasValidTarget1;
    bool hasValidTarget2;

    private bool tutorialHintUsed = false;


    private Transform lockedWheel;
    private RaycastHit lockedHit;
    private float lockTimer = 0f;

    [SerializeField] private float lockGraceTime = 0.8f;

    private AimHintPopupUI aimHintPopup;

    private bool coinsLocked = false;


    [Header("Tutorial Aim Hint")]
    [SerializeField] private int tutorialSuccessLimit = 2;
    [SerializeField] private LineRenderer hintLine;
    [SerializeField] private LayerMask wheelLayer;
    private int successfulJumpCount = 0;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        cameraObj = Camera.main;
        aimHintPopup = FindFirstObjectByType<AimHintPopupUI>();
        count = 0;

    }

    void Update()
    {
        if (!isAlive) return;

        Vector3 direction = head.up;
        direction = Quaternion.AngleAxis(5f, head.right) * direction;

        RaycastHit hit0, hit1, hit2;
        bool h0 = Physics.SphereCast(head.position, castRadius, direction, out hit0, castDistance,wheelLayer);
        bool h1 = Physics.SphereCast(head1.position, castRadius, direction, out hit1, castDistance,wheelLayer);
        bool h2 = Physics.SphereCast(head2.position, castRadius, direction, out hit2, castDistance, wheelLayer);

        bool hasAnyHit = false;
        RaycastHit finalHit = new RaycastHit();
        Vector3 finalOrigin = Vector3.zero;

       
        if (h0)
        {
            hasAnyHit = true;
            finalHit = hit0;
            finalOrigin = head.position;
        }

        if (h1 && (!hasAnyHit || hit1.distance < finalHit.distance))
        {
            hasAnyHit = true;
            finalHit = hit1;
            finalOrigin = head1.position;
        }

        if (h2 && (!hasAnyHit || hit2.distance < finalHit.distance))
        {
            hasAnyHit = true;
            finalHit = hit2;
            finalOrigin = head2.position;
        }

        if (hasAnyHit)
        {
            lockedWheel = finalHit.collider.transform.root;
            lockedHit = finalHit;
            lockTimer = lockGraceTime;
            Vector3 dir = (lockedHit.point - finalOrigin).normalized;
            float dist = Vector3.Distance(finalOrigin, lockedHit.point);

            if (!tutorialHintConsumed && lockedWheel != null)
            {
                ShowHintLine(finalOrigin, lockedHit.point);
                if (aimHintPopup != null && aimHintPopup.gameObject.activeSelf)
                    aimHintPopup.Show();
            }
           
           
            else
            {
                HideHintLine();
            }
            //if (!coinsLocked)
            //{
            //    CoinSpawner.Instance?.SpawnCoinsAlongRay(
            //      finalOrigin,
            //      dir,
            //      dist
            //  );
            //    coinsLocked = true;
            //}
            
        }
        else
        {
            lockTimer -= Time.deltaTime;
            if (lockTimer <= 0f)
                lockedWheel = null;
            HideHintLine();
        }
       
        if (hasAnyHit)
        {
            Debug.DrawRay(finalOrigin, direction * finalHit.distance, Color.green);
            HitWheel = finalHit.collider.transform;
        }
        else
        {
            Debug.DrawRay(head.position, direction * castDistance, Color.red);
            Debug.DrawRay(head1.position, direction * castDistance, Color.red);
            Debug.DrawRay(head2.position, direction * castDistance, Color.red);
        }

        if (EventSystem.current.IsPointerOverGameObject()) return;

        
        if (Input.GetMouseButtonDown(0) && !inputLocked)
        {
            tutorialHintConsumed = true;
            HideHintLine();
            inputLocked = true;
            if (lockedWheel != null)
            {
                rayHitPoint = lockedHit.point;
                rayHitNormal = lockedHit.normal;
                rayHitWheel = lockedWheel;

                JumpToTarget();
                count++;
            }
            else
            {
              
                transform.SetParent(null);

                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.AddForce(transform.position / 3f, ForceMode.Impulse);
                count++;
                StartCoroutine(CheckForGameOver());
               
            }

        }
    }



    void JumpToTarget()
    {
        hasJumped = true;
        jumpResolved = false;

        GetComponent<Collider>().enabled = true;
        GameFlowController.Instance.OnPlayerJumped();
        transform.parent.gameObject.layer = 0;
        foreach (Transform t in transform.parent)
        {
            t.gameObject.layer = 0;
        }
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

        successfulJumpCount++;
        HideHintLine();
        CoinSpawner.Instance?.ClearCoins();
        coinsLocked = false;

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

        StartCoroutine(SmoothAttach(wheel, HitWheel));

    }
    IEnumerator InputLockCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        inputLocked = false;
    }

    public static void ResetTutorial()
    {
        tutorialHintConsumed = false;
    }

    public static void DisableTutorial()
    {
        tutorialHintConsumed = true;
    }

    void ShowHintLine(Vector3 start, Vector3 end)
    {
        if (hintLine == null) return;

        hintLine.enabled = true;

        Vector3 dir = (end - start).normalized;
        float hintLength = 25.0f;
        Vector3 startPos = start + dir * 0.25f;

        Vector3 endPos = startPos + dir * hintLength;

        hintLine.SetPosition(0, startPos);
        hintLine.SetPosition(1, endPos);
    }

    void HideHintLine()
    {
        if (hintLine != null && hintLine.enabled)
            hintLine.enabled = false;
    }

    IEnumerator SmoothAttach(Transform wheel, Transform magnet)
    {
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;


        transform.SetParent(wheel, false);
        transform.localPosition = new Vector3(-8.14f, -0.1900f, -2.62997f);

        transform.localRotation = Quaternion.Euler(-99f, 134.2f, -66.3f);
        transform.localScale = new Vector3(1f, 1f, 1f);

        yield break;
    }
    public void ResetTutorialState()
    {
        tutorialHintUsed = false;
        successfulJumpCount = 0;
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
        coinsLocked = false;
CoinSpawner.Instance?.ClearCoins();

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
        CoinSpawner.Instance?.ClearCoins();
        coinsLocked = false;

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
                while (Filler.IsPowerActive)
                    yield return null;
            }

            yield return new WaitForSeconds(1f);
            DieImmediate();
        }
    }

    public void DisableAimHintPopup()
    {
        if (aimHintPopup != null)
            aimHintPopup.gameObject.SetActive(false);
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


