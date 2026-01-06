using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float followTime = 3f; 
    private Transform target;
    public Transform player_;
    private bool follow = false;
    public float smoothTime = 0.25f; 
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        
    }

    void LateUpdate()
    {
        if (GameFlowController.Instance.IsStarted)
        {
            PlayerCube player = FindAnyObjectByType<PlayerCube>();
            player_ = player.transform;
        }
       
        Vector3 targetPosition = new Vector3(player_.position.x, 0, player_.position.z) + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

    }
}
