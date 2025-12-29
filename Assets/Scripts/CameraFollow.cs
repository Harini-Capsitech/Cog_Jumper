using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Target (Assigned at Runtime)")]
    public Transform target;     

    [Header("Camera Settings")]
    public float smoothSpeed = 5f;
    public Vector3 followOffset = new Vector3(0f, 3f, -6f); 

    [Header("Follow Options")]
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;

    private void LateUpdate()
    {
        if (target == null) return;  

        Vector3 desiredPos = target.position + followOffset;
        Vector3 currentPos = transform.position;

        Vector3 finalPos = new Vector3(
            followX ? desiredPos.x : currentPos.x,
            followY ? desiredPos.y : currentPos.y,
            followZ ? desiredPos.z : currentPos.z
        );

        transform.position = Vector3.Lerp(currentPos, finalPos, smoothSpeed * Time.deltaTime);
    }

   
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}