using UnityEngine;
using System.Collections;

public class AimHintPopupUI : MonoBehaviour
{
    [SerializeField] private float autoHideTime = 3f;
    private Coroutine hideRoutine;

    // DO NOT disable in Awake
    void OnEnable()
    {
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
