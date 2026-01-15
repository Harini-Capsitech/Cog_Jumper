using System.Collections;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    //
    [SerializeField] private ParticleSystem hitEffect;

    private float speed = 500f;
    private float lifeTime = 5f;

    private bool isMoving = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    private float distance = 800f;
    private float travelDis = 0f;
    private Vector3 direction;

    void Start()
    {
        Destroy(gameObject,30f);
    }

    void Update()
    {
        if (!isMoving)
        {
            StopCoroutine(leftMoveRoutine());
            StopCoroutine(rightMoveRoutine());
        }

        if (isMoving && (isMovingLeft || isMovingRight))
        {
            if ((distance - travelDis) >= 0.03f)
            {
                travelDis += Time.deltaTime * speed;
                if (isMovingRight) transform.position += new Vector3(Time.deltaTime * speed, 0f, 0f);
                else
                {
                    transform.position -= new Vector3(Time.deltaTime * speed, 0f, 0f);
                }
            }
            else
            {
                if (isMovingRight)
                {
                    isMovingRight = false;
                    StartCoroutine(leftMoveRoutine());
                }
                else
                {
                    isMovingLeft = false;
                    StartCoroutine(rightMoveRoutine());
                }
            }

        }
    }

    IEnumerator rightMoveRoutine()
    {
        distance = 800f;
        speed = 500f;
        travelDis = 0f;
        yield return new WaitForSeconds(0.3f);
        isMovingRight = true;
    }

    IEnumerator leftMoveRoutine()
    {
        distance = 800f;
        speed = 2000f;
        travelDis = 0f;
        yield return new WaitForSeconds(0.3f);
        isMovingLeft = true;
    }

    public void Fire(Vector3 moveDirection)
    {
        isMoving = true;
        isMovingRight = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCube"))
            // 🔥 Spawn hit VFX
            if (hitEffect != null)
            {
                ParticleSystem vfx = Instantiate(
                    hitEffect,
                    other.transform.position,
                    Quaternion.identity
                );
                vfx.Play();
            }

        // Respect power mode
        if (Filler.IsPowerActive)
            return;

        //other.GetComponent<PlayerCube>()?.DieImmediate();
        // ⏱ Delay death
        StartCoroutine(DelayedKill(other.GetComponent<PlayerCube>()));
    }

    IEnumerator DelayedKill(PlayerCube player)
    {
        if (player == null) yield break;

        yield return new WaitForSeconds(2f); // ⏱ adjust delay here

        // Check again (player might already be saved / attached)
        if (Filler.IsPowerActive) yield break;

        player.DieImmediate();
    }

}


