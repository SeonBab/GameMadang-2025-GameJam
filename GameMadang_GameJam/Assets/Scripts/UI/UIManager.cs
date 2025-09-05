using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private PauseMenu pauseMenu;

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
