//using UnityEngine;

//public class LaserProjectile : MonoBehaviour
//{
//    [Header("Laser Movement")]
//    public float speed = 8f;

//    private Vector3 direction;

//    void Start()
//    {
//        // Destroy laser after some time
//        Destroy(gameObject, 5f);
//    }

//    void Update()
//    {
//        // Move laser like a bullet
//        transform.Translate(direction * speed * Time.deltaTime, Space.World);
//    }

//    // Called by spawner
//    public void SetDirection(Vector3 newDirection)
//    {
//        direction = newDirection.normalized;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            other.GetComponent<PlayerCube>()?.DieImmediate();
//            Destroy(gameObject);
//        }
//    }
//}
//using UnityEngine;

//public class LaserProjectile : MonoBehaviour
//{
//    public float speed = 8f;

//    private Vector3 targetPos;
//    private bool hasTarget = false;

//    void Update()
//    {
//        if (!hasTarget) return;

//        // Move towards target
//        transform.position = Vector3.MoveTowards(
//            transform.position,
//            targetPos,
//            speed * Time.deltaTime
//        );

//        // Reached target → destroy
//        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
//        {
//            Destroy(gameObject);
//        }
//    }

//    // Called by spawner
//    public void SetTarget(Vector3 target)
//    {
//        targetPos = target;
//        hasTarget = true;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            other.GetComponent<PlayerCube>()?.DieImmediate();
//            Destroy(gameObject);
//        }
//    }
//}
//using UnityEngine;

//public class LaserProjectile : MonoBehaviour
//{
//    [Header("Movement")]
//    public float speed = 8f;

//    private Vector3 startPos;
//    private Vector3 targetPos;
//    private bool movingToTarget = true;

//    void Update()
//    {
//        if (movingToTarget)
//        {
//            transform.position = Vector3.MoveTowards(
//                transform.position,
//                targetPos,
//                speed * Time.deltaTime
//            );

//            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
//            {
//                // Reverse direction
//                Vector3 temp = startPos;
//                startPos = targetPos;
//                targetPos = temp;
//            }
//        }
//    }

//    // Called by LaserSpawner
//    public void Initialize(Vector3 start, Vector3 target)
//    {
//        startPos = start;
//        targetPos = target;
//        transform.position = startPos;
//        movingToTarget = true;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            other.GetComponent<PlayerCube>()?.DieImmediate();
//        }
//    }
//}



using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;

    private Vector3 targetPos;
    private bool isMoving = false;

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // Destroy when reached right point
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            Destroy(gameObject);
        }
    }

    // Called by spawner
    public void Fire(Vector3 startPos, Vector3 endPos)
    {
        transform.position = startPos;
        targetPos = endPos;
        isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCube>()?.DieImmediate();
        }
    }
}
