using UnityEngine;

using System.Collections.Generic;

public class GameFlowController : MonoBehaviour

{

    public static GameFlowController Instance;

    public WheelSpawner wheelSpawner;

    public Transform wheelsParent;

    public GameObject playerCubePrefab;

    private int score = 0;

    [HideInInspector] public List<GameObject> wheels = new List<GameObject>();

    private PlayerCube player;

    private int wheelIndex = 0;

    public GameObject currentWheel;


    private Camera mainCam;


    public float cameraMoveSpeed = 4f;

    void Awake()

    {

        Instance = this;

        mainCam = Camera.main;

    }

    void Start()

    {

        player = FindAnyObjectByType<PlayerCube>();

        if (player == null && playerCubePrefab != null)

        {

            GameObject playerObj = Instantiate(playerCubePrefab, Vector3.zero, Quaternion.identity);

            

            player = playerObj.GetComponent<PlayerCube>();

            player.gameObject.name = "PlayerCube";

        }

        if (player == null)

        {

            Debug.LogError("No PlayerCube found!");

            return;

        }

        SpawnInitialWheels();

    }

    void SpawnInitialWheels()

    {

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

            Debug.LogError("No Magnet tags found!");

            return;

        }

        Transform chosenMagnet = magnets[Random.Range(0, magnets.Count)];

        player.AttachToMagnet(wheel.transform, chosenMagnet);

    }

    public void OnPlayerJumped()

    {

        if (currentWheel != null)

        {

            SetWheelGapTriggers(currentWheel, false);

        }

    }

    public void PlayerLanded(GapTrigger gap)

    {

        score += 5;

        if (GameplayScoreUI.Instance != null)

            GameplayScoreUI.Instance.UpdateScore(score);

        Transform wheelTransform = gap.transform.parent;

        GameObject landedWheel = wheelTransform.parent.gameObject;


        player.AttachToMagnet(wheelTransform, gap.snapMagnet);

        currentWheel = landedWheel;

        SetWheelGapTriggers(landedWheel, false);


        GameObject nextWheel = wheelSpawner.SpawnWheel(wheelIndex++, wheelsParent);

        wheels.Add(nextWheel);

        player.targetWheel = nextWheel.transform;

        SetWheelGapTriggers(nextWheel, true);

        MoveCameraBetweenWheels(landedWheel, nextWheel);

        CleanupOldWheels();

    }


    void MoveCameraBetweenWheels(GameObject current, GameObject next)

    {

        float midZ = (current.transform.position.z + next.transform.position.z) * 0.5f;

        Vector3 targetPos = new Vector3(

            mainCam.transform.position.x,

            mainCam.transform.position.y,

            midZ

        );

        StopAllCoroutines();

        StartCoroutine(SmoothMoveCamera(targetPos));

    }


    System.Collections.IEnumerator SmoothMoveCamera(Vector3 targetPos)

    {

        while (Vector3.Distance(mainCam.transform.position, targetPos) > 0.01f)

        {

            mainCam.transform.position = Vector3.Lerp(

                mainCam.transform.position,

                targetPos,

                Time.deltaTime * cameraMoveSpeed

            );

            yield return null;

        }

        mainCam.transform.position = targetPos;

    }

    void SetWheelGapTriggers(GameObject wheel, bool value)

    {

        GapTrigger[] gaps = wheel.GetComponentsInChildren<GapTrigger>(true);

        foreach (var gap in gaps)

        {

            gap.EnableTrigger(value);

        }

    }

    void CleanupOldWheels()

    {

        while (wheels.Count > 3)

        {

            GameObject oldWheel = wheels[0];

            wheels.RemoveAt(0);

            Destroy(oldWheel);

        }

    }

    public void GameOver()

    {

        Time.timeScale = 0f;

        wheelsParent.gameObject.SetActive(false);

        player.gameObject.SetActive(false);

        AppManager.instance.GameOver();

        GameOverUI.Instance.Show(score);

        player.GetComponent<Rigidbody>().isKinematic = false;

        player.GetComponent<Rigidbody>().mass = 0.5f;

        Debug.Log("Game Over!");

    }

}

