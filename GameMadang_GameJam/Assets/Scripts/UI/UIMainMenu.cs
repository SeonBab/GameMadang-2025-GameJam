using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEditor;

public class UIMainMenu : MonoBehaviour
{
    public static UIMainMenu Instance
    {  get; private set; }

    [SerializeField] Button startButton;
    [SerializeField] Button quitGameButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료 호출");

        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
