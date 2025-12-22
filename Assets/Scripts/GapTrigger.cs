using UnityEngine;

public class GapTrigger : MonoBehaviour
{
    private bool isActive = false;

    public void EnableTrigger(bool value)
    {
        isActive = value;
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (!isActive) return;
        if (!other.CompareTag("PlayerCube")) return;

        GameFlowController.Instance.PlayerLanded(this);
    }
}
