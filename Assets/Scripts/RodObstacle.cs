using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

enum cubeDirection {
    state1,
    state2
}
public class RodObstacle : MonoBehaviour
{

    private bool isMoving = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    private float distance = 30f;
    private float travelDis = 0f;

     private float MoveSpeed = 50f;
    [SerializeField] private cubeDirection direction;

    private void Update()
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
                travelDis += Time.deltaTime * MoveSpeed;
                if (isMovingRight) transform.position += new Vector3(Time.deltaTime * MoveSpeed, 0f, 0f);
                else
                {
                    transform.position -= new Vector3(Time.deltaTime * MoveSpeed, 0f, 0f);
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

    private void Awake()
    {
        ActivateMovingCube();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCube"))
        {
            GameFlowController.Instance.FinalGameOver();
        }
    }

    IEnumerator rightMoveRoutine()
    {
        distance = 30f;
        travelDis = 0f;
        yield return new WaitForSeconds(3f);
        isMovingRight = true;
    }

    IEnumerator leftMoveRoutine()
    {
        distance = 30f;
        travelDis = 0f;
        yield return new WaitForSeconds(3f);
        isMovingLeft = true;
    }

    public void ActivateMovingCube()
    {
        isMoving = true;
        if(direction == cubeDirection.state1) isMovingRight = true;
        else
        {
            isMovingLeft = true;
        }
    }
}
