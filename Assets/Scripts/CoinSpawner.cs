using UnityEngine;
using System.Collections.Generic;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance;

    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float spacing =5f;
    [SerializeField] private float startOffset = 1.0f;
    [SerializeField] private float endOffsetFromWheel = 1.2f; // IMPORTANT

    private readonly List<GameObject> spawnedCoins = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnCoinsAlongRay(
        Vector3 origin,
        Vector3 direction,
        float hitDistance
    )
    {
        ClearCoins();

        // ✅ Clamp coin path BEFORE the wheel
        float usableDistance = hitDistance - endOffsetFromWheel;
        if (usableDistance <= startOffset) return;

        int coinCount = Mathf.FloorToInt((usableDistance - startOffset) / spacing);

        Vector3 startPos = origin + direction * startOffset;

        for (int i = 0; i < coinCount; i++)
        {
            Vector3 pos = startPos + direction * (i * spacing);

            GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity);

            // Optional: face camera slightly (looks nicer in 3D)
            coin.transform.forward = Camera.main.transform.forward;

            spawnedCoins.Add(coin);
        }
    }

    public void ClearCoins()
    {
        for (int i = 0; i < spawnedCoins.Count; i++)
        {
            if (spawnedCoins[i] != null)
                Destroy(spawnedCoins[i]);
        }
        spawnedCoins.Clear();
    }
}
