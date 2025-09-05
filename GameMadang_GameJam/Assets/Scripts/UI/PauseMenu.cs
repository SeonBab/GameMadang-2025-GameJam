using System;
using UnityEngine;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
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

        public void ReturnToGame()
        {
            Hide();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}