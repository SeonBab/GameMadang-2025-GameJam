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
            GameManager.RestartGame();
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