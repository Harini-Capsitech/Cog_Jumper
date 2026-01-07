using UnityEngine;
using System.Collections;

public class PlayerJumpEffect : MonoBehaviour
{
    public ParticleSystem jumpEffectPrefab;
    public Transform[] edgePoints;

    public void PlayAttachEffect(float delay = 0.1f)
    {
        StartCoroutine(PlayWithDelay(delay));
    }

    private IEnumerator PlayWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (Transform point in edgePoints)
        {
            ParticleSystem fx = Instantiate(
                jumpEffectPrefab,
                point.position,
                Quaternion.identity,
                gameObject.transform
            );

            fx.Play();
            Destroy(fx.gameObject, 2f);
        }
    }
}

