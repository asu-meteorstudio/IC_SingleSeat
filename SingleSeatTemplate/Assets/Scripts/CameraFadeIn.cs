using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraFadeIn : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 3.0f;

    private void Start()
    {
        StartCoroutine(WakeUpEffect());
    }

    IEnumerator WakeUpEffect()
    {
        Color startColor = fadeImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float startTime = Time.time;

        while (Time.time - startTime < fadeDuration)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / fadeDuration;
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeImage.color = endColor;
    }
}
