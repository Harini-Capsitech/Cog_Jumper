using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GameFlowController : MonoBehaviour
{
    private int bestScore = 0;
    private const string BEST_SCORE_KEY = "BEST_SCORE";

    public static GameFlowController Instance;

    [Header("References")]
    public WheelSpawner wheelSpawner;
    public Transform wheelsParent;
    public GameObject playerCubePrefab;
    public bool isHintConsumed = false;

    [Header("Camera")]
    public bool followPlayerAfterGameOver = false;


    [Header("Gameplay")]
    public int score = 0;
    private int wheelIndex = 0;
    public GameObject currentWheel;
    [Header("Gameplay Background")]


    [Header("Combo Settings")]
    [SerializeField] private int comboTriggerScore = 35;
    private const int COMBO_HITS = 3;

    //laser
    [Header("Laser Obstacle Settings")]
    [SerializeField] private LaserSpawner laserSpawner;


    private bool comboActive = false;
    private bool comboTriggered = false;
    private int scoreMultiplier = 1;
    private int comboRemainingHits = 0;
    [Header("Rod Obstacle Settings")]
    [SerializeField] private GameObject rodPrefabA;
    [SerializeField] private GameObject rodPrefabB;

    private GameObject saveMeButton;
    private bool saveMeUsed = false;
    public int saveMeCount = 0;
    public static bool IsGameplayInputBlocked = false;

    private enum ObstacleType { None, Laser, Rod }
    private ObstacleType lastSpawnedObstacle = ObstacleType.None;

    [SerializeField] private int obstacleStartScore = 30;


    private int nextObstacleAllowedScore;
    private int nextRodSpawnScore;

    [HideInInspector] public List<GameObject> wheels = new List<GameObject>();

    private PlayerCube player;
    private Camera mainCam;

    public bool IsStarted = false;
    public static float CurrentWheelSpeed;
    private const float BASE_WHEEL_SPEED = 100f;
    private const int SCORE_STEP = 25;
    private const float SPEED_INCREMENT = 5f;
    private Vector3 startPos;
    private Quaternion rot;

    private bool perfectShown = false;
    void Awake()
    {
        Debug.Log("[GFC] Awake");
        Time.timeScale = 1f;
        Instance = this;
        mainCam = Camera.main;
        startPos = mainCam.transform.position;
        rot = mainCam.transform.rotation;
        followPlayerAfterGameOver = false;

        bestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        CurrentWheelSpeed = BASE_WHEEL_SPEED;
        Debug.Log("[GFC] Initialized. BaseSpeed = " + CurrentWheelSpeed);
    }


    void Start()
    {
        Debug.Log("[GFC] Start called");
        FindSaveMeButton();
        StartCoroutine(LogGameStartWhenFirebaseReady());
        Debug.Log("start");

        if (player == null && playerCubePrefab != null)
        {
            Debug.Log("[GFC] Instantiating PlayerCube");
            GameObject obj = Instantiate(playerCubePrefab, Vector3.zero, Quaternion.identity);
            player = obj.GetComponent<PlayerCube>();
            IsStarted = true;
        }
        IsStarted = true;
        Invoke("ResetGame", 0.1f);
    }

    void LateUpdate()
    {
        if (!followPlayerAfterGameOver || player == null) return;

    }

    void FindSaveMeButton()
    {
        GameObject canvas = GameObject.Find("Canvas (Environment)");
        if (canvas == null) return;

        Transform gameOverP = canvas.transform.Find("GameOverP");
        if (gameOverP == null) return;

        Transform btn = gameOverP.transform.Find("SaveMeButton");
        if (btn == null) return;

        saveMeButton = btn.gameObject;
        saveMeButton.SetActive(false);
    }


    IEnumerator LogGameStartWhenFirebaseReady()
    {
        while (!FirebaseInitializer.IsFirebaseReady)
            yield return null;

        Debug.Log("✅ Firebase ready, logging game_start");
        AnalyticsLogger.LogGameStart();
    }

    public void ResetGame()
    {
        Debug.Log("========== RESET GAME ==========");

        StopAllCoroutines();
        //PlayerCube.ResetTutorial();

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
        perfectShown = false;
        isHintConsumed = false;   
        comboActive = false;
        comboTriggered = false;
        scoreMultiplier = 1;
        comboRemainingHits = 0;
        nextObstacleAllowedScore = obstacleStartScore;
        lastSpawnedObstacle = ObstacleType.None;
        //laser
        Debug.Log("laser emitted");
        //

        currentWheel = null;
        wheels.Clear();

        CurrentWheelSpeed = BASE_WHEEL_SPEED;
        Debug.Log("[GFC] Cleared wheels list");

        if (wheelsParent != null)
        {
            wheelsParent.gameObject.SetActive(true);
            foreach (Transform child in wheelsParent)
                Destroy(child.gameObject);
        }

        player.ResetPlayerState();
        EnableAimHintPopup();

        player.transform.SetParent(null);
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
        Debug.Log("[GFC] Spawning initial wheels...");
        SpawnInitialWheels();
    }

    void EnableAimHintPopup()
    {
        AimHintPopupUI popup = FindFirstObjectByType<AimHintPopupUI>();
        if (popup != null)
            popup.gameObject.SetActive(true);
    }

    void SpawnInitialWheels()
    {
        Debug.Log("========== SpawnInitialWheels ==========");

        followPlayerAfterGameOver = false;
        GameObject firstWheel = wheelSpawner.SpawnWheel(wheelIndex++, wheelsParent);
        wheels.Add(firstWheel);
        Debug.Log("[GFC] First wheel: " + firstWheel.name);
        GameObject secondWheel = wheelSpawner.SpawnWheel(wheelIndex++, wheelsParent);
        wheels.Add(secondWheel);
        Debug.Log("[GFC] Second wheel: " + secondWheel.name);
        StartCoroutine(InitializePlayerAfterFrame(firstWheel, secondWheel));
    }

    System.Collections.IEnumerator InitializePlayerAfterFrame(GameObject firstWheel, GameObject secondWheel)
    {
        yield return null;
        Debug.Log("[GFC] Initializing Player After Frame");
        AttachPlayerToFirstWheel(firstWheel);
        currentWheel = firstWheel;
        Debug.Log("[GFC] CurrentWheel = " + currentWheel.name);

        player.targetWheel = secondWheel.transform;
        Debug.Log("[GFC] TargetWheel = " + player.targetWheel.name);

        SetWheelGapTriggers(firstWheel, false);
        SetWheelGapTriggers(secondWheel, true);
    }
    void AttachPlayerToFirstWheel(GameObject wheel)
    {
        GapTrigger[] gaps = wheel.GetComponentsInChildren<GapTrigger>(true);
        if (gaps.Length == 0) return;

        GapTrigger randomGap = gaps[Random.Range(0, gaps.Length)];
        Debug.Log("[GFC] Attaching player to first wheel: " + wheel.name);
        player.AttachToMagnet(wheel.transform, randomGap.snapMagnet);
    }

    public void OnPlayerJumped()
    {
        Debug.Log("[GFC] Jumped from: " + (currentWheel != null ? currentWheel.name : "NULL"));
        if (currentWheel != null)
            SetWheelGapTriggers(currentWheel, false);
    }

    public void PlayerLanded(GapTrigger gap)
    {

        int baseScore = 5;
        score += baseScore * scoreMultiplier;
        Filler.instance.FillSlider();
        bool comboShownThisHit = false;
        bool popupTriggered = false;

        if (!comboTriggered && score >= comboTriggerScore)
        {
            comboTriggered = true;
            scoreMultiplier = 2;
            comboRemainingHits = COMBO_HITS;

            ComboX2Popup.Instance?.Show();
            comboShownThisHit = true;
            AnalyticsLogger.LogComboActivated();
            popupTriggered = true;

        }

        else if (!perfectShown && score > 20)
        {
            perfectShown = true;

            PerfectPopup.Instance?.Show();
            AnalyticsLogger.LogPerfectJump();

            popupTriggered = true;
        }

        if (scoreMultiplier > 1)
        {
            comboRemainingHits--;

            if (comboRemainingHits <= 0)
            {
                scoreMultiplier = 1;
            }
        }
        x2ButtonUI x2 = FindFirstObjectByType<x2ButtonUI>();
        if (x2 == null)
        {
            Debug.Log("x2 is null");
        }
        if (x2 != null)
        {
            x2.TryActivateX2(score);
        }

        GameplayScoreUI.Instance?.UpdateScore(score);
        if (score % 25 == 0)
        {
            Debug.Log("Milestone");
            AnalyticsLogger.LogScoreMilestone(score);
        }

        UpdateWheelSpeed();


        //Transform wheelTransform = gap.transform.parent;
        //GameObject landedWheel = wheelTransform.parent.gameObject;
        //new prefab code
        //Transform wheelTransform = gap.transform.parent;
        //GameObject landedWheel = wheelTransform.gameObject;
        //updated
        Debug.Log("[GFC] Landed via gap: " + gap.name);
        WheelRotation wheel = gap.GetComponentInParent<WheelRotation>();

        if (wheel == null)
        {
            Debug.LogError("WheelRotation not found in PlayerLanded!");
            return;
        }

        Transform wheelTransform = wheel.transform;
        GameObject landedWheel = wheelTransform.gameObject;

        Debug.Log("[GFC] LandedWheel detected: " + landedWheel.name);

        player.AttachToMagnet(wheelTransform, gap.snapMagnet);
        currentWheel = landedWheel;
        Debug.Log("[GFC] CurrentWheel updated to: " + currentWheel.name);
        SetWheelGapTriggers(landedWheel, false);

        GameObject nextWheel = wheelSpawner.SpawnWheel(wheelIndex++, wheelsParent);
        Debug.Log("[GFC] Spawned NextWheel: " + nextWheel.name);
        wheels.Add(nextWheel);
        

        if (score >= obstacleStartScore && score % 15 == 0)
        {
            Debug.Log("obstacle spawned at a score " + score);
            if (lastSpawnedObstacle == ObstacleType.None)
            {
                SpawnTwoRodsBetweenWheels(
                    currentWheel.transform,
                    nextWheel.transform
                );
                lastSpawnedObstacle = ObstacleType.Rod;
            }

            else if (lastSpawnedObstacle == ObstacleType.Rod)
            {
                laserSpawner.SpawnLaserBetweenWheels(
                    currentWheel.transform,
                    nextWheel.transform
                );
                lastSpawnedObstacle = ObstacleType.Laser;
            }
            else
            {
                lastSpawnedObstacle = ObstacleType.None;
            }
        }

        player.targetWheel = nextWheel.transform;
        Debug.Log("[GFC] New TargetWheel = " + player.targetWheel.name);
        SetWheelGapTriggers(nextWheel, true);
        CleanupOldWheels();

    }
    void SpawnTwoRodsBetweenWheels(Transform wheelA, Transform wheelB)
    {

        if (rodPrefabA == null) return;
        int offset = 5;
        Vector3 midPoint = (wheelA.position + wheelB.position) / 2f;

        Instantiate(rodPrefabA, new Vector3(midPoint.x - offset, midPoint.y, midPoint.z), Quaternion.identity, wheelsParent);
        Instantiate(rodPrefabB, new Vector3(midPoint.x + offset, midPoint.y, midPoint.z), Quaternion.identity, wheelsParent);
        AnalyticsLogger.LogRodSpawned(score);
    }

    void UpdateWheelSpeed()
    {
        int steps = score / SCORE_STEP;
        CurrentWheelSpeed = BASE_WHEEL_SPEED + steps * SPEED_INCREMENT;
    }

    void SetWheelGapTriggers(GameObject wheel, bool value)
    {
        GapTrigger[] gaps = wheel.GetComponentsInChildren<GapTrigger>(true);
        Debug.Log($"[GFC] SetWheelGapTriggers → {wheel.name} | Active = {value} | Count = {gaps.Length}");
        foreach (var gap in gaps)
            gap.EnableTrigger(value);
    }

    
    void CleanupOldWheels()
{
    while (wheels.Count > 3)
    {
        if (player != null && player.transform.IsChildOf(wheels[0].transform))
            return;

        Destroy(wheels[0]);
        wheels.RemoveAt(0);
    }
}

    GameObject GetNextWheelAfter(GameObject wheel)
    {
        int index = wheels.IndexOf(wheel);
        if (index >= 0 && index + 1 < wheels.Count)
            return wheels[index + 1];

        return null;
    }

    public void FinalGameOver()
    {

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BEST_SCORE_KEY, bestScore);
            PlayerPrefs.Save();
        }
        Debug.Log("game ends");
        Time.timeScale = 0f;
        AppManager.instance.disableGameLogic();
        AppManager.instance.isSaveMeActive = true;
        GoogleMobileAdsDemoScript.Instance.ShowInterstitialOnRestart(() =>
        {

        });
        AppManager.instance.GameOver();
        AnalyticsLogger.LogGameOver(score, bestScore);

        mainCam.transform.parent = null;
        if (player != null)
            Destroy(player.gameObject);
        player = null;

        GameOverUI.Instance.Show(score, bestScore);
        if (!saveMeUsed && saveMeButton != null)
            saveMeButton.SetActive(true);
    }

    public bool CanUseSaveMe()
    {
        if (saveMeUsed) return false;
        if (saveMeCount >= 2) return false;
        return true;
    }

    public void SaveMe()

    {
        if (saveMeCount >= 2) return;
        //saveMeCount++;

        Debug.Log("save me activated 2");

        saveMeUsed = true;

        Time.timeScale = 1f;

        AppManager.instance.isSaveMeActive = true;
        AnalyticsLogger.LogSaveMeUsed(score);


        if (saveMeButton != null)

            saveMeButton.SetActive(false);

        AppStateManager.Instance.SetGameplay();

        GameOverUI.Instance.Hide();

        GameObject obj = Instantiate(playerCubePrefab, Vector3.zero, Quaternion.identity);

        player = obj.GetComponent<PlayerCube>();

        AppManager.instance.enableGameLogic();
        //PlayerCube.DisableTutorial();

        //player.DisableAimHintPopup();


        player.ResetJumpState();

        AttachPlayerToFirstWheel(currentWheel);

        GameObject nextWheel = GetNextWheelAfter(currentWheel);

        if (nextWheel != null)

        {

            player.targetWheel = nextWheel.transform;

            SetWheelGapTriggers(nextWheel, true);

        }

        followPlayerAfterGameOver = false;

        GameplayScoreUI.Instance?.UpdateScore(score);

        Debug.Log("SaveMe complete: gameplay fully restored at score " + score);

    }

    public int GetScore()
    {
        return score;
    }


    public void ApplyScoreMultiplierOnce(int multiplier)
    {
        score *= multiplier;
        GameplayScoreUI.Instance?.UpdateScore(score);
    }

}