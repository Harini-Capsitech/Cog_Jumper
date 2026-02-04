using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCube"))
        {
            CoinManager.Instance.AddCoin(value);
            gameObject.SetActive(false);
            Destroy(gameObject,1f);
        }
    }

    private void Update()
    {
        // Smooth spin
        //transform.Rotate(Vector3.up * 180f * Time.deltaTime, Space.World);
    }
}
