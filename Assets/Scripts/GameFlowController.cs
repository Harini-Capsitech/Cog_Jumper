using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Firebase.Analytics;


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
    [Header("Gameplay Background")]
   

    [Header("Combo Settings")]
    [SerializeField] private int comboTriggerScore = 35;
    private const int COMBO_HITS = 3;
    

    //laser
    [Header("Laser Obstacle Settings")]
    [SerializeField] private LaserSpawner laserSpawner;
    [SerializeField] private int firstLaserScore = 50;
    [SerializeField] private int laserScoreInterval = 70;

    private int nextLaserSpawnScore;
    
    private bool comboActive = false;
    private bool comboTriggered = false;
    private int scoreMultiplier = 1;
    private int comboRemainingHits = 0;
    [Header("Rod Obstacle Settings")]
    [SerializeField] private GameObject rodPrefabA;   
   [SerializeField] private GameObject rodPrefabB; 
    [SerializeField] private int firstRodScore = 35;
    [SerializeField] private int rodScoreInterval = 30;

    private GameObject saveMeButton;
    private bool saveMeUsed = false;
    public int saveMeCount = 0;
    public static bool IsGameplayInputBlocked = false;


    private int nextRodSpawnScore;
    
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

    private bool perfectShown = false;
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
      
    }
    

    void Start()
    {
        FindSaveMeButton();
        Debug.Log("start");
        LogGameStart();
        if (player == null && playerCubePrefab != null)
        {
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
        perfectShown = false;

        comboActive = false;
        comboTriggered = false;
        scoreMultiplier = 1;
        comboRemainingHits = 0;

        nextRodSpawnScore = firstRodScore;
        //laser
        nextLaserSpawnScore = firstLaserScore;
        Debug.Log("laser emitted");
        //

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
       // Transform[] allChildren = wheel.transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("Magnet"))
                magnets.Add(child);
        }

        if (magnets.Count == 0)
        {
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
        
        int baseScore = 5;
        score += baseScore * scoreMultiplier;
        Filler.instance.FillSlider();
        if (!comboTriggered && score >= comboTriggerScore)
        {
            comboTriggered = true;
            scoreMultiplier = 2;
            comboRemainingHits = COMBO_HITS;
            Debug.Log("Combo event");
            LogComboActivated();
            ComboX2Popup.Instance?.Show();

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
            LogScoreMilestone();
        }

        UpdateWheelSpeed();

        if (!perfectShown && score > 20)
        {
            perfectShown = true;
            Debug.Log("Perfect Shown");
            LogPerfectJump();

            PerfectPopup.Instance?.Show();
        }
        //Transform wheelTransform = gap.transform.parent;
        //GameObject landedWheel = wheelTransform.parent.gameObject;
        //new prefab code
        Transform wheelTransform = gap.transform.parent;
        GameObject landedWheel = wheelTransform.gameObject;




        player.AttachToMagnet(wheelTransform, gap.snapMagnet);
        currentWheel = landedWheel;

        SetWheelGapTriggers(landedWheel, false);

        GameObject nextWheel = wheelSpawner.SpawnWheel(wheelIndex++, wheelsParent);
        wheels.Add(nextWheel);

        
        if (score >= nextLaserSpawnScore && laserSpawner != null)
        {
            laserSpawner.SpawnLaserBetweenWheels(
                currentWheel.transform,
                nextWheel.transform
            );

            nextLaserSpawnScore += laserScoreInterval;
        }

        if (score >= nextRodSpawnScore)
        {
            SpawnTwoRodsBetweenWheels(currentWheel.transform, nextWheel.transform);
            nextRodSpawnScore += rodScoreInterval;
        }
        player.targetWheel = nextWheel.transform;

        SetWheelGapTriggers(nextWheel, true);
        CleanupOldWheels();
       
    }
    void SpawnTwoRodsBetweenWheels(Transform wheelA, Transform wheelB)
    {
        Debug.Log("Rods are spawned");
        LogRodSpawned();
        Debug.Log("Rods are spawned");
        if (rodPrefabA == null ) return;
        int offset = 5;
        Vector3 midPoint = (wheelA.position + wheelB.position) / 2f;

        Instantiate(rodPrefabA, new Vector3(midPoint.x - offset, midPoint.y, midPoint.z), Quaternion.identity, wheelsParent);
        Instantiate(rodPrefabB, new Vector3(midPoint.x + offset,midPoint.y,midPoint.z), Quaternion.identity, wheelsParent);
    }
    
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
        LogGameOver();
        
        Time.timeScale = 0f;
        AppManager.instance.disableGameLogic();
        AppManager.instance.isSaveMeActive = true;
        AppManager.instance.GameOver();
        
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

        if (saveMeButton != null)

            saveMeButton.SetActive(false);

        AppStateManager.Instance.SetGameplay();

        GameOverUI.Instance.Hide();

        GameObject obj = Instantiate(playerCubePrefab, Vector3.zero, Quaternion.identity);

        player = obj.GetComponent<PlayerCube>();

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
    // 2x 
    // 🔒 Read-only access to score
    public int GetScore()
    {
        return score;
    }

    // 🔥 Controlled x2 application
    public void ApplyScoreMultiplierOnce(int multiplier)
    {
        score *= multiplier;
        GameplayScoreUI.Instance?.UpdateScore(score);
    }

    void LogGameStart()
    {
        if (!FirebaseInitializer.IsFirebaseReady) return;

        FirebaseAnalytics.LogEvent("game_start");

    }

    void LogGameOver()
    {
        if (!FirebaseInitializer.IsFirebaseReady) return;

        FirebaseAnalytics.LogEvent(
            "game_over",
            new Parameter("score", score),
            new Parameter("best_score", bestScore)
        );
    }

    void LogScoreMilestone()
    {
        if (!FirebaseInitializer.IsFirebaseReady) return;

        FirebaseAnalytics.LogEvent(
            "score_milestone",
            new Parameter("score", score)
        );
    }

    void LogComboActivated()
    {
        if (!FirebaseInitializer.IsFirebaseReady) return;

        FirebaseAnalytics.LogEvent("combo_x2_activated");
    }

    void LogRodSpawned()
    {
        if (!FirebaseInitializer.IsFirebaseReady) return;

        FirebaseAnalytics.LogEvent(
            "rod_spawned",
            new Parameter("score", score)
        );
    }

    void LogPerfectJump()
    {
        if (!FirebaseInitializer.IsFirebaseReady) return;

        FirebaseAnalytics.LogEvent("perfect_jump");
    }





}




