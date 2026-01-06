using UnityEngine;
using System.Collections.Generic;

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

    [HideInInspector] public List<GameObject> wheels = new List<GameObject>();

    private PlayerCube player;
    private Camera mainCam;

    public bool IsStarted =false;
    public static float CurrentWheelSpeed;
    private const float BASE_WHEEL_SPEED = 150f;
    private const int SCORE_STEP = 25;
    private const float SPEED_INCREMENT = 25f;
    private Vector3 startPos;
    private Quaternion rot;
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
        if (player == null && playerCubePrefab != null)
        {
            GameObject obj = Instantiate(playerCubePrefab, Vector3.zero, Quaternion.identity);
            player = obj.GetComponent<PlayerCube>();
            IsStarted = true;
        }
        ResetGame();
    }

    void LateUpdate()
    {
       
        if (!followPlayerAfterGameOver || player == null) return;

    }

    
    public void ResetGame()
    {
        StopAllCoroutines();
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

        player.targetWheel = nextWheel.transform;

        SetWheelGapTriggers(nextWheel, true);
        CleanupOldWheels();
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
        player.gameObject.SetActive(false);
        //player.enabled = false;
        //player.GetComponent<Rigidbody>().isKinematic = true;

        GameOverUI.Instance.Show(score,bestScore);
    }
}
