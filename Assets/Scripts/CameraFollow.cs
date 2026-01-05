//using UnityEngine;
//using System.Collections;

//public class GameOverCameraFollow : MonoBehaviour
//{
//    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);
//    [SerializeField] private float followSpeed = 5f;
//    [SerializeField] private float followTime = 3f; // 2–5 seconds

//    private Transform target;
//    private bool follow = false;

//    void LateUpdate()
//    {
//        if (!follow || target == null) return;

//        Vector3 targetPos = target.position + offset;
//        transform.position = Vector3.Lerp(
//            transform.position,
//            targetPos,
//            followSpeed * Time.deltaTime
//        );

//        transform.LookAt(target);
//    }

//    public void StartFollow(Transform cube)
//    {
//        target = cube;
//        StartCoroutine(FollowRoutine());
//    }

//    private IEnumerator FollowRoutine()
//    {
//        follow = true;
//        yield return new WaitForSeconds(followTime);
//        follow = false;
//    }
//}
