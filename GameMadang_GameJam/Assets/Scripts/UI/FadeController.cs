using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    [SerializeField] public float FadeInTime { get; private set; }
    [SerializeField] public float FadeOutTime { get; private set; }

    private Image fadeImage;

    private void Start()
    {
        Transform fadeCanvas = transform.Find("FadeCanvas");
        Transform fadeImageTransform = fadeCanvas.transform.Find("FadeImage");
        fadeImage = fadeImageTransform.GetComponent<Image>();
    }

    public void PlayFadeIn()
    {

    }

    public void PlayFadeOut()
    {

    }

    private IEnumerator FadeInCorutine()
    {
        yield return null;
    }

    private IEnumerator FadeOutCorutine()
    {
        yield return null;
    }
}
