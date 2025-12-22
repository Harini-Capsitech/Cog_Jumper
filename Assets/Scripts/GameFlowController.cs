//using UnityEngine;
//using System.Collections.Generic;

//public class GameFlowController : MonoBehaviour
//{
//    public static GameFlowController Instance;

//    public WheelSpawner wheelSpawner;
//    public Transform wheelsParent;
//    public GameObject playerCubePrefab;

//    [HideInInspector] public List<GameObject> wheels = new List<GameObject>();

//    private PlayerCube player;
//    private int wheelIndex = 0;

//    public GameObject currentWheel;

//    void Awake()
//    {
//        Instance = this;
//    }

//    void Start()
//    {
//        player = FindAnyObjectByType<PlayerCube>();

//        if (player == null && playerCubePrefab != null)
//        {
//            GameObject playerObj = Instantiate(playerCubePrefab, Vector3.zero, Quaternion.identity);
//            player = playerObj.GetComponent<PlayerCube>();
//            player.gameObject.name = "PlayerCube";
//        }

//        if (player == null)
//        {
//            Debug.LogError("No PlayerCube found!");
//            return;
//        }

//        SpawnInitialWheels();
//    }

//    void SpawnInitialWheels()
//    {
//        GameObject firstWheel = wheelSpawner.SpawnWheelAtPoint(wheelIndex++, wheelsParent);
//        wheels.Add(firstWheel);

//        GameObject secondWheel = wheelSpawner.SpawnWheelAtPoint(wheelIndex++, wheelsParent);
//        wheels.Add(secondWheel);

//        StartCoroutine(InitializePlayerAfterFrame(firstWheel, secondWheel));
//    }

//    System.Collections.IEnumerator InitializePlayerAfterFrame(GameObject firstWheel, GameObject secondWheel)
//    {
//        yield return null;

//        AttachPlayerToFirstWheel(firstWheel);
//        currentWheel = firstWheel;
//        player.targetWheel = secondWheel.transform;

//        SetWheelGapTriggers(firstWheel, false);
//        SetWheelGapTriggers(secondWheel, true);
//    }

//    void AttachPlayerToFirstWheel(GameObject wheel)
//    {
//        List<Transform> magnets = new List<Transform>();
//        Transform[] allChildren = wheel.GetComponentsInChildren<Transform>();

//        foreach (Transform child in allChildren)
//        {
//            if (child.CompareTag("Magnet"))
//                magnets.Add(child);
//        }

//        if (magnets.Count == 0)
//        {
//            Debug.LogError("No Magnet tags found!");
//            return;
//        }

//        Transform chosenMagnet = magnets[Random.Range(0, magnets.Count)];
//        player.AttachToMagnet(wheel.transform, chosenMagnet);
//    }


//    public void OnPlayerJumped()
//    {
//        if (currentWheel != null)
//        {

//            SetWheelGapTriggers(currentWheel, false);
//        }

//    }



//    public void PlayerLanded(GapTrigger magnet)
//    {
//        Transform wheelTransform = magnet.transform.parent;
//        GameObject landedWheel = wheelTransform.parent.gameObject;


//        player.AttachToMagnet(wheelTransform, magnet.transform);
//        currentWheel = landedWheel;

//        SetWheelGapTriggers(landedWheel, false);
//        GameObject nextWheel = wheelSpawner.SpawnWheelAtPoint(wheelIndex++, wheelsParent);
//        wheels.Add(nextWheel);


//        player.targetWheel = nextWheel.transform;


//        SetWheelGapTriggers(nextWheel, true);
//        CleanupOldWheels();
//    }




//    void SetWheelGapTriggers(GameObject wheel, bool value)
//    {
//        GapTrigger[] gaps = wheel.GetComponentsInChildren<GapTrigger>(true);
//        foreach (var gap in gaps)
//        {
//            gap.EnableTrigger(value);
//        }
//    }

//    void CleanupOldWheels()
//    {
//        while (wheels.Count > 3)
//        {
//            GameObject oldWheel = wheels[0];
//            wheels.RemoveAt(0);
//            Destroy(oldWheel);
//            //settleWheels();
//        }
//    }
//    //void settleWheels()
//    //{
//    //    foreach (GameObject wheel in wheels)
//    //    {
//    //        wheel.transform.position -= new Vector3(0f, 0f, 25f);
//    //    }
//    //}


//    public void GameOver()
//    {
//        Debug.Log("Game Over!");
//    }
//}

using UnityEngine;
using System.Collections.Generic;

public class GameFlowController : MonoBehaviour
{
    public static GameFlowController Instance;

    public WheelSpawner wheelSpawner;
    public Transform wheelsParent;
    public GameObject playerCubePrefab;

    [HideInInspector] public List<GameObject> wheels = new List<GameObject>();

    private PlayerCube player;
    private int wheelIndex = 0;

    public GameObject currentWheel;

    // ================= CAMERA SETTINGS (NEW) =================
    [Header("Camera Follow")]
    [SerializeField] private float cameraZOffset = -20f;
    // =========================================================

    void Awake()
    {
        Instance = this;
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
        GameObject firstWheel = wheelSpawner.SpawnWheelAtPoint(wheelIndex++, wheelsParent);
        wheels.Add(firstWheel);

        GameObject secondWheel = wheelSpawner.SpawnWheelAtPoint(wheelIndex++, wheelsParent);
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

        // ---------- CAMERA INITIAL POSITION (NEW) ----------
        MoveCameraToWheel(firstWheel);
        // -----------------------------------------------
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

    public void PlayerLanded(GapTrigger magnet)
    {
        Transform wheelTransform = magnet.transform.parent;
        GameObject landedWheel = wheelTransform.parent.gameObject;

        player.AttachToMagnet(wheelTransform, magnet.transform);
        currentWheel = landedWheel;

        // ================= CAMERA MOVE (NEW) =================
        MoveCameraToWheel(landedWheel);
        // ====================================================

        SetWheelGapTriggers(landedWheel, false);

        GameObject nextWheel = wheelSpawner.SpawnWheelAtPoint(wheelIndex++, wheelsParent);
        wheels.Add(nextWheel);

        player.targetWheel = nextWheel.transform;
        SetWheelGapTriggers(nextWheel, true);

        CleanupOldWheels();
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

    // ================= CAMERA FUNCTION (NEW) =================
    void MoveCameraToWheel(GameObject wheel)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 camPos = cam.transform.position;
        camPos.z = wheel.transform.position.z + cameraZOffset;
        cam.transform.position = camPos;
    }
    // =========================================================

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }
}
