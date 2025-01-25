using UnityEngine;
using System.Collections;

public class LightFader : MonoBehaviour
{
    // public Light directionalLight;
    // public float maxIntensity = 50f;
    // private Coroutine fadeCoroutine;

    // void Start()
    // {
    //     if (directionalLight == null)
    //     {
    //         directionalLight = GetComponent<Light>();
    //     }
    //     directionalLight.intensity = 0;
    // }

    // public void StartLightFade(float duration)
    // {
    //     Debug.Log("START LIGHT FADE");
    //     if (fadeCoroutine != null)
    //     {
    //         StopCoroutine(fadeCoroutine);
    //     }
    //     fadeCoroutine = StartCoroutine(FadeLightIntensity(duration));
    // }

    // private IEnumerator FadeLightIntensity(float duration)
    // {
    //     float halfDuration = duration / 2f;
    //     float elapsedTime = 0f;

    //     while (elapsedTime < halfDuration)
    //     {
    //         directionalLight.intensity = Mathf.Lerp(0, maxIntensity, elapsedTime / halfDuration);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     elapsedTime = 0f;
    //     while (elapsedTime < halfDuration)
    //     {
    //         directionalLight.intensity = Mathf.Lerp(maxIntensity, 0, elapsedTime / halfDuration);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     directionalLight.intensity = 0;
    //     fadeCoroutine = null;
    // }
}
