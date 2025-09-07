using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Save;
using UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {  get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogError("두 개 이상의 게임 매니저가 존재합니다.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(OnStartPreRender());
    }

    public static void EndingGame(float fadeOutDelay)
    {
        Debug.Log("엔딩 호출");

        // 페이드 아웃
        UIManager.Instance.PlayFadeOut(fadeOutDelay);

        //TODO
        // 페이드 아웃 이후 엔딩 관련 연출 필요

    }

    public static void RestartGame(float sceneLoadDelay)
    {
        UIManager.Instance.PlayFadeOut(sceneLoadDelay);

        // 페이드 아웃 시간 대기
        UIManager.Instance.StartCoroutine(DelayedSceneLoad());
        IEnumerator DelayedSceneLoad()
        {
            yield return new WaitForSeconds(sceneLoadDelay);

            // 씬 다시 호출
            SceneManager.LoadScene("Level");
        }
    }

    // 모든 Start함수가 호출 된 뒤 플레이어 캐릭터 위치 로드
    private IEnumerator OnStartPreRender()
    {
        yield return new WaitForEndOfFrame();

        UIManager.Instance.PlayFadeIn();
        SaveManager.Instance.Load();
    }
}
