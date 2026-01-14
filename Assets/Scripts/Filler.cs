using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Filler : MonoBehaviour
{
    [SerializeField] private Slider filler;
    [SerializeField] private float increment = 0.1f;
    [SerializeField] private float powerDuration = 10f;
    private float value = 0f;
    private bool powerRunning = false;

    public static Filler instance;
    public static bool IsPowerActive { get; private set; } 

    private void Awake()
    {
        instance = this;
        IsPowerActive = false;
    }

    public void FillSlider()
    {
        if (powerRunning) return;
        value += increment;
        value = Mathf.Min(value, 1f);
        filler.value = value;
        if (value >= 1f)
        {
            StartCoroutine(ActivatePowerMode());
        }
    }
    private IEnumerator ActivatePowerMode()
    {
        powerRunning = true;
        IsPowerActive = true;

        Debug.Log("🔥 POWER MODE ACTIVATED");

        // Optional: add VFX / UI glow / sound here

        yield return new WaitForSeconds(powerDuration);

        IsPowerActive = false;
        powerRunning = false;

        value = 0f;
        filler.value = 0f;

        Debug.Log("⏱ POWER MODE ENDED");
    }
}


