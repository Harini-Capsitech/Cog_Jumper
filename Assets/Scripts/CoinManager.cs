using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    public int totalCoins = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddCoin(int amount)
    {
        totalCoins += amount;
        Debug.Log("Coins: " + totalCoins);
        // Update UI here later
    }
}
