using UnityEngine;
using System.Collections;

public class PerfectPopup : MonoBehaviour
{
    public static PerfectPopup Instance;

    [SerializeField] private float showDuration = 1f;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        StopAllCoroutines();
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0f;

        // Scale In
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 6f;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(showDuration);

        // Scale Out
        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 6f;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
