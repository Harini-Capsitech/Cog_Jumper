using UnityEngine;
using System.Collections;

public class ComboX2Popup : MonoBehaviour
{
    public static ComboX2Popup Instance;

    [SerializeField] private float showDuration = 1.5f;

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

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 6f;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(showDuration);
        gameObject.SetActive(false);
    }
}
