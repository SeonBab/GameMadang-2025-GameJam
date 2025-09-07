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
            Destroy(gameObject);
        }
    }

    void Start()
    {
        OnGameStart(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        SceneManager.sceneLoaded += OnGameStart;
    }

    public static void EndingGame(float fadeOutDelay)
    {
        Debug.Log("엔딩 호출");

        // 페이드 아웃
        UIManager.instance.PlayFadeOut(fadeOutDelay);

        // 플레이어 캐릭터에 대한 모든 조작이 불가능하도록 한다.
        InputHandler.OnRemoveInputCallbacks.Invoke();

        // 페이드 아웃 이후 엔딩의 시퀀스를 재생한다.
        UIManager.instance.StartCoroutine(DelayedPlayEndingSequence());
        IEnumerator DelayedPlayEndingSequence()
        {
            yield return new WaitForSecondsRealtime(fadeOutDelay);

            UIManager.instance.PlayEndingSequence();
        }

        // 모든 엔딩의 시퀀스를 재생한 이후 메인메뉴로 씬을 이동한다.
        EndingSequenceController.OnSequenceFinished += MoveSceneMainMenu;

        // 세이브 포인트 초기화
        SaveManager.ResetSavePointIndex();
    }

    public static void RestartGame(float sceneLoadDelay)
    {
        UIManager.instance.PlayFadeOut(sceneLoadDelay);

        // 페이드 아웃 시간 대기
        UIManager.instance.StartCoroutine(DelayedSceneLoad());
        IEnumerator DelayedSceneLoad()
        {
            yield return new WaitForSecondsRealtime(sceneLoadDelay);

            // 씬 다시 호출
            SceneManager.LoadScene("Level");
        }
    }

    public static void MoveSceneMainMenu()
    {
        SceneManager.LoadScene("MainMenu");

        instance.StartCoroutine(NextFrame());
        IEnumerator NextFrame()
        {
            AsyncOperation asycLoad = SceneManager.LoadSceneAsync("MainMenu");

            while (!asycLoad.isDone)
            {
                yield return null;
            }

            Destroy(UIManager.instance.gameObject);
            Destroy(SaveManager.instance.gameObject);
            Destroy(instance.gameObject);
        }
    }

    // 모든 Start함수가 호출 된 뒤 플레이어 캐릭터 위치 로드
    private void OnGameStart(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitializeScene());
        IEnumerator InitializeScene()
        {
            yield return new WaitForFixedUpdate();

            UIManager.instance.PlayFadeIn();
            SaveManager.instance.Load();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnGameStart;

        EndingSequenceController.OnSequenceFinished -= MoveSceneMainMenu;

        if (instance == this)
        {
            instance = null;
        }
    }
}
