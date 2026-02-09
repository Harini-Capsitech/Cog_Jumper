using UnityEngine;
using System.Collections;

public class AimHintPopupUI : MonoBehaviour
{
    [SerializeField] private float autoHideTime = 3f;
    private Coroutine hideRoutine;


    void OnEnable()
    {
        if (GameFlowController.Instance.isHintConsumed)
        {
            gameObject.SetActive(false);
            return;
        }
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(AutoHide());
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(autoHideTime);
        gameObject.SetActive(false);
    }
}

