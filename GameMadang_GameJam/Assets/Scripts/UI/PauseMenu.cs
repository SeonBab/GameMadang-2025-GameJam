using System;
using Save;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] Button moveToLastSavePointButton;
        [SerializeField] Button resumeButton;
        [SerializeField] Button quitGameButton;

        void Awake()
        {
            moveToLastSavePointButton.onClick.AddListener(MoveToLastSavePoint);
            resumeButton.onClick.AddListener(ResumeGame);
            quitGameButton.onClick.AddListener(QuitGame);
        }

        void Start()
        {
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        public void MoveToLastSavePoint()
        {
            GameManager.RestartGame(3f);

            // 플레이어 캐릭터에 대한 모든 조작이 불가능하도록 한다.
            InputHandler.OnRemoveInputCallbacks.Invoke();

            Hide();
        }

        public void ResumeGame()
        {
            Hide();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}