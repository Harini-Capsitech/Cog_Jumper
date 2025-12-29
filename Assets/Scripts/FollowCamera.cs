using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    Vector3 localPos;
    Quaternion localRot;

    void Start()
    {
        localPos = transform.localPosition;
        localRot = transform.localRotation;
    }

    void LateUpdate()
    {
        transform.localPosition = localPos;
        transform.localRotation = localRot;
    }
}
