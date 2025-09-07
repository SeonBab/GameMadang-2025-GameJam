using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    [SerializeField] private float fadeInTime = 1f;
    public float FadeInTime => fadeInTime;

    // 씬을 로드하는 과정에 필요한 시간으로 대체
    // 기획적인 부분으로 인해 코드를 제거하지 않고 남겨둠.
    //[SerializeField] private float fadeOutTime = 1f;
    //public float FadeOutTime => fadeOutTime;

    private Image fadeImage;

    private void Start()
    {
        Transform fadeCanvas = transform.Find("FadeCanvas");
        Transform fadeImageTransform = fadeCanvas.transform.Find("FadeImage");
        fadeImage = fadeImageTransform.GetComponent<Image>();
    }

    public IEnumerator FadeInCorutine()
    {
        float elapsedTime = 0f;

        // 시작 색
        Color fadeColor = fadeImage.color;
        fadeColor.a = 1f;
        fadeImage.color = fadeColor;

        // 색 갱신
        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;

            fadeColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeInTime);
            fadeImage.color = fadeColor;

            yield return null;
        }

        // 최종 보정
        fadeColor.a = 0f;
        fadeImage.color = fadeColor;
    }

    public IEnumerator FadeOutCorutine(float sceneLoadDelay)
    {
        float elapsedTime = 0f;

        // 시작 색
        Color fadeColor = fadeImage.color;
        fadeColor.a = 0f;
        fadeImage.color = fadeColor;

        // 색 갱신
        while (elapsedTime < sceneLoadDelay)
        {
            elapsedTime += Time.deltaTime;

            fadeColor.a = Mathf.Lerp(0f, 1f, elapsedTime / sceneLoadDelay);
            fadeImage.color = fadeColor;
            yield return null;
        }

        // 최종 보정
        fadeColor.a = 1f;
        fadeImage.color = fadeColor;
    }
}
