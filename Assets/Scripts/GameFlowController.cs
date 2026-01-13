using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    private int bestScore = 0;
    private const string BEST_SCORE_KEY = "BEST_SCORE";

    public static GameFlowController Instance;

    [Header("References")]
    public WheelSpawner wheelSpawner;
    public Transform wheelsParent;
    public GameObject playerCubePrefab;

    [Header("Camera")]
    public float cameraMoveSpeed = 4f;
    public bool followPlayerAfterGameOver = false;
    public float followSmoothSpeed = 5f;
    public Vector3 playerCameraOffset = new Vector3(0, 2f, -6f);

    [Header("Gameplay")]
    private int score = 0;
    private int wheelIndex = 0;
    public GameObject currentWheel;

    [Header("Laser")]
    public LaserSpawner laserSpawner;


    [HideInInspector] public List<GameObject> wheels = new List<GameObject>();

    private PlayerCube player;
    private Camera mainCam;

    public bool IsStarted = false;
    public static float CurrentWheelSpeed;
    private const float BASE_WHEEL_SPEED = 100f;
    private const int SCORE_STEP = 25;
    private const float SPEED_INCREMENT = 25f;
    private Vector3 startPos;
    private Quaternion rot;

    //public float shakeDuration = 0.08f;
    //public float shakeStrength = 0.1f;
    //public Camera cameraObj;
    void Awake()
    {
        Time.timeScale = 1f;
        Instance = this;
        mainCam = Camera.main;
        startPos = mainCam.transform.position;
        rot = mainCam.transform.rotation;
        followPlayerAfterGameOver = false;

        bestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        CurrentWheelSpeed = BASE_WHEEL_SPEED;
        //cameraObj = Camera.main;
    }

    void Start()
    {
        if (player == null && playerCubePrefab != null)
        {
            GameObject obj = Instantiate(playerCubePrefab, Vector3.zero, Quaternion.identity);
            player = obj.GetComponent<PlayerCube>();
            IsStarted = true;
        }
        IsStarted = true;
        Invoke("ResetGame", 0.1f); //ResetGame();
    }

    void LateUpdate()
    {
        if (!followPlayerAfterGameOver || player == null) return;

    }


    public void ResetGame()
    {
      
        StopAllCoroutines();
        if (player == null)
        {
            GameObject obj = Instantiate(playerCubePrefab, Vector3.zero, Quaternion.identity);
            player = obj.GetComponent<PlayerCube>();
        }
        mainCam.transform.position = startPos;
        mainCam.transform.rotation = rot;
        Time.timeScale = 1f;
        followPlayerAfterGameOver = false;

        score = 0;
        wheelIndex = 0;
        currentWheel = null;
        wheels.Clear();

        CurrentWheelSpeed = BASE_WHEEL_SPEED;

        if (wheelsParent != null)
        {
            wheelsParent.gameObject.SetActive(true);
            foreach (Transform child in wheelsParent)
                Destroy(child.gameObject);
        }


        player.ResetPlayerState();
        player.transform.SetParent(null);
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.identity;

        SpawnInitialWheels();
    }

    void SpawnInitialWheels()
    {
        followPlayerAfterGameOver = false;
        GameObject firstWheel = wheelSpawner.SpawnWheel(wheelIndex++, wheelsParent);
        wheels.Add(firstWheel);

        GameObject secondWheel = wheelSpawner.SpawnWheel(wheelIndex++, wheelsParent);
        wheels.Add(secondWheel);

        StartCoroutine(InitializePlayerAfterFrame(firstWheel, secondWheel));
    }

    System.Collections.IEnumerator InitializePlayerAfterFrame(GameObject firstWheel, GameObject secondWheel)
    {
        yield return null;

        AttachPlayerToFirstWheel(firstWheel);
        currentWheel = firstWheel;
        player.targetWheel = secondWheel.transform;

        SetWheelGapTriggers(firstWheel, false);
        SetWheelGapTriggers(secondWheel, true);
    }

    void AttachPlayerToFirstWheel(GameObject wheel)
    {
        List<Transform> magnets = new List<Transform>();
        Transform[] allChildren = wheel.GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("Magnet"))
                magnets.Add(child);
        }

        if (magnets.Count == 0)
        {
            Debug.LogError("❌ No Magnet found on wheel!");
            return;
        }

        Transform chosenMagnet = magnets[Random.Range(0, magnets.Count)];
        player.AttachToMagnet(wheel.transform, chosenMagnet);
    }
    public void OnPlayerJumped()
    {
        if (currentWheel != null)
            SetWheelGapTriggers(currentWheel, false);
    }

    public void PlayerLanded(GapTrigger gap)
    {
        score += 5;
        GameplayScoreUI.Instance?.UpdateScore(score);
        UpdateWheelSpeed();

        Transform wheelTransform = gap.transform.parent;
        GameObject landedWheel = wheelTransform.parent.gameObject;

        player.AttachToMagnet(wheelTransform, gap.snapMagnet);
        currentWheel = landedWheel;

        SetWheelGapTriggers(landedWheel, false);

        GameObject nextWheel = wheelSpawner.SpawnWheel(wheelIndex++, wheelsParent);
        wheels.Add(nextWheel);


        //laser script 
        // 🔥 SPAWN LASER BETWEEN CURRENT & NEXT WHEEL
        if (laserSpawner != null)
        {
            laserSpawner.SpawnLaserBetween(
                landedWheel.transform,
                nextWheel.transform
            );
        }
        ////end 

        player.targetWheel = nextWheel.transform;

        SetWheelGapTriggers(nextWheel, true);
        CleanupOldWheels();
        //StartCoroutine(ShakeCoroutine());
    }

    //IEnumerator ShakeCoroutine()

    //{

    //    float elapsed = 0f;

    //    Vector3 beforePos = cameraObj.transform.position;

    //    while (elapsed < shakeDuration)

    //    {

    //        float x = Random.Range(-1f, 1f) * shakeStrength;

    //        float y = Random.Range(-1f, 1f) * shakeStrength;

    //        cameraObj.transform.localPosition = cameraObj.transform.position + new Vector3(x, y, 0);

    //        elapsed += Time.deltaTime;

    //        yield return null;

    //    }

    //    cameraObj.transform.localPosition = beforePos;

    //}


    void UpdateWheelSpeed()
    {
        int steps = score / SCORE_STEP;
        CurrentWheelSpeed = BASE_WHEEL_SPEED + steps * SPEED_INCREMENT;
    }

    void SetWheelGapTriggers(GameObject wheel, bool value)
    {
        GapTrigger[] gaps = wheel.GetComponentsInChildren<GapTrigger>(true);
        foreach (var gap in gaps)
            gap.EnableTrigger(value);
    }

    void CleanupOldWheels()
    {
        while (wheels.Count > 3)
        {
            Destroy(wheels[0]);
            wheels.RemoveAt(0);
        }
    }


    public void PreGameOverCleanup()
    {
        foreach (GameObject wheel in wheels)
            if (wheel != null)
                Destroy(wheel);

        wheels.Clear();

        if (wheelsParent != null)
            wheelsParent.gameObject.SetActive(false);

        followPlayerAfterGameOver = true;
    }

    public void FinalGameOver()
    {
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BEST_SCORE_KEY, bestScore);
            PlayerPrefs.Save();
        }

        Time.timeScale = 0f;
        AppManager.instance.GameOver();
        mainCam.transform.parent = null;
        Destroy(player.gameObject);
        player = null;


        GameOverUI.Instance.Show(score, bestScore);
    }
}