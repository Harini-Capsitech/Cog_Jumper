//using UnityEngine;
//using System.Collections;

//public class LaserSpawner : MonoBehaviour
//{
//    public GameObject laserPrefab;
//    public Transform[] spawnPoints;
//    public float fireInterval = 1.5f;

//    void Start()
//    {
//        StartCoroutine(FireRoutine());
//    }

//    IEnumerator FireRoutine()
//    {
//        while (true)
//        {
//            foreach (Transform spawn in spawnPoints)
//            {
//                FireLaser(spawn);
//            }

//            yield return new WaitForSeconds(fireInterval);
//        }
//    }

//    void FireLaser(Transform spawn)
//    {
//        GameObject laser = Instantiate(
//            laserPrefab,
//            spawn.position,
//            spawn.rotation,
//            spawn.parent   // 🔥 keeps it inside gameplay
//        );

//        LaserProjectile projectile = laser.GetComponent<LaserProjectile>();
//        if (projectile != null)
//        {
//            projectile.SetDirection(Vector3.right); // always horizontal
//        }
//    }

//}
//using UnityEngine;
//using System.Collections;

//public class LaserSpawner : MonoBehaviour
//{
//    public GameObject laserPrefab;
//    public Transform[] spawnPoints; // size = 2
//    public float fireInterval = 1.5f;

//    void Start()
//    {
//        StartCoroutine(FireRoutine());
//    }

//    IEnumerator FireRoutine()
//    {
//        while (true)
//        {
//            if (spawnPoints.Length >= 2)
//            {
//                // Laser from 0 → 1
//                FireLaser(spawnPoints[0], spawnPoints[1]);

//                // Laser from 1 → 0
//                FireLaser(spawnPoints[1], spawnPoints[0]);
//            }

//            yield return new WaitForSeconds(fireInterval);
//        }
//    }

//    void FireLaser(Transform from, Transform to)
//    {
//        GameObject laser = Instantiate(
//            laserPrefab,
//            from.position,
//            Quaternion.identity,
//            from.parent
//        );

//        LaserProjectile projectile = laser.GetComponent<LaserProjectile>();
//        if (projectile != null)
//        {
//            projectile.SetTarget(to.position);
//        }
//    }
//}

//using UnityEngine;

//public class LaserSpawner : MonoBehaviour
//{
//    public GameObject laserPrefab;
//    public float offset = 4f;
//    public Transform wheelsParent;

//    public void SpawnLaserBetween(Transform wheelA, Transform wheelB)
//    {
//        if (wheelA == null || wheelB == null) return;

//        // Midpoint
//        Vector3 midPoint = (wheelA.position + wheelB.position) * 0.5f;

//        // Left & Right positions
//        Vector3 leftPos = midPoint - Vector3.right * offset;
//        Vector3 rightPos = midPoint + Vector3.right * offset;

//        GameObject laser = Instantiate(
//            laserPrefab,
//            leftPos,
//            Quaternion.identity,
//            wheelsParent
//        );

//        LaserProjectile projectile = laser.GetComponent<LaserProjectile>();
//        if (projectile != null)
//        {
//            projectile.Initialize(leftPos, rightPos);
//        }
//    }
//}
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform wheelsParent;

    [Header("Laser Geometry")]
    public float horizontalRange = 8f; // total width from left to right

    public void SpawnLaserBetween(Transform wheelA, Transform wheelB)
    {
        if (wheelA == null || wheelB == null) return;

        // 1️⃣ Midpoint between wheels
        Vector3 midPoint = (wheelA.position + wheelB.position) * 0.5f;

        // 2️⃣ Compute LEFT and RIGHT world points
        Vector3 leftPoint = midPoint - Vector3.right * (horizontalRange * 0.5f);
        Vector3 rightPoint = midPoint + Vector3.right * (horizontalRange * 0.5f);

        // 3️⃣ Spawn laser at LEFT
        GameObject laser = Instantiate(
            laserPrefab,
            leftPoint,
            Quaternion.identity,
            wheelsParent
        );

        // 4️⃣ Fire laser LEFT → RIGHT
        LaserProjectile projectile = laser.GetComponent<LaserProjectile>();
        if (projectile != null)
        {
            projectile.Fire(leftPoint, rightPoint);
        }
    }
}
