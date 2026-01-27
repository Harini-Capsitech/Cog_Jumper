using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform wheelsParent;

    private float laserOffset = 120f; // distance from left-most wheel

    //public void SpawnLaserBetweenWheels(Transform wheelA, Transform wheelB)
    //{
    //    if (laserPrefab == null || wheelA == null || wheelB == null) return;

    //    // Midpoint ONLY for Y & Z
    //    Vector3 midPoint = (wheelA.position + wheelB.position) * 0.5f;

    //    // Left-most wheel X
    //    float leftMostX = Mathf.Min(wheelA.position.x, wheelB.position.x);

    //    // Laser spawn position (AT-MOST LEFT)
    //    Vector3 spawnPos = new Vector3(
    //        midPoint.x - laserOffset,
    //        midPoint.y,
    //        midPoint.z
    //    );

    //    GameObject laser = Instantiate(
    //        laserPrefab,
    //        spawnPos,
    //        Quaternion.identity,
    //        wheelsParent
    //    );

    //    // Fire laser LEFT → RIGHT
    //    LaserProjectile projectile = laser.GetComponent<LaserProjectile>();
    //    if (projectile != null)
    //    {
    //        projectile.Fire(Vector3.right);
    //    }
    //}

    public GameObject SpawnLaserBetweenWheels(Transform wheelA, Transform wheelB)
    {
        if (laserPrefab == null || wheelA == null || wheelB == null) return null;

        Vector3 midPoint = (wheelA.position + wheelB.position) * 0.5f;
        float leftMostX = Mathf.Min(wheelA.position.x, wheelB.position.x);

        Vector3 spawnPos = new Vector3(
            midPoint.x - laserOffset,
            midPoint.y,
            midPoint.z
        );

        GameObject laser = Instantiate(
            laserPrefab,
            spawnPos,
            Quaternion.identity,
            wheelsParent
        );

        LaserProjectile projectile = laser.GetComponent<LaserProjectile>();
        if (projectile != null)
            projectile.Fire(Vector3.right);

        return laser;
    }

}
