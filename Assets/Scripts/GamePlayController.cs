using UnityEngine;

public class WheelGameplayController : MonoBehaviour
{
    public static WheelGameplayController Instance;

    public Transform[] wheelMagnets;   // assign magnet points in order
    private int currentIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    public void JumpToNextWheel(PlayerJump player)
    {
        currentIndex++;

        if (currentIndex >= wheelMagnets.Length)
        {
            // You can spawn more wheels or loop
            Debug.Log("No more wheels!");
            return;
        }

        player.SetNextMagnet(wheelMagnets[currentIndex]);
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
        // show UI / stop game
    }
}
