using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] public FadeController fadeController { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            fadeController = GetComponent<FadeController>();
        }

        public void PlayFadeIn()
        {
            
            StartCoroutine(fadeController.FadeInCorutine());
        }

        public void PlayFadeOut(float sceneLoadDelay)
        {
            StartCoroutine(fadeController.FadeOutCorutine(sceneLoadDelay));
        }

        public void TogglePauseMenu(InputAction.CallbackContext ctx)
        {
            if (pauseMenu.gameObject.activeSelf)
            {
                pauseMenu.Hide();
            }
            else
            {
                pauseMenu.Show();
            }
        }
    }
}
