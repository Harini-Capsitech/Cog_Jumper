using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public GameObject playerPrefab;

    private GameObject player;

    void Start()
    {
        Invoke(nameof(SpawnPlayerFromWheel), 0.1f); // wait until wheel spawns
    }

    void SpawnPlayerFromWheel()
    {
        // 1️ Find first spawned wheel
        GameObject wheel = GameObject.FindWithTag("Wheel");

        if (wheel == null)
        {
            Debug.LogError(" Wheel not found!");
            return;
        }

        // 2️ Find Ps inside Spawn
        Transform ps = wheel.transform.Find("Spawn/Magnet 1");
        WheelGameplayController wgc = wheel.GetComponent<WheelGameplayController>();
        
       Transform t = wgc.wheelMagnets[0].transform;
        if (ps == null)
        {
            Debug.LogError(" Ps not found under Spawn!");
            return;
        }

        // 3️ Spawn Player
        player = Instantiate(playerPrefab, t);
        

        player.transform.position = ps.position;
        player.transform.rotation = ps.rotation;
        player.transform.localScale = new Vector3(5f, 5f, 5f);

        // 4️ Physics setup
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;

        Debug.Log(" Player spawned at Ps");
    }
}
