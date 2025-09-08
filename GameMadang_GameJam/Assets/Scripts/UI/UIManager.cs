using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance { get; private set; }

        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] public FadeController fadeController { get; private set; }
        [SerializeField] public EndingSequenceController endingSequenceController { get; private set; }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            Transform child = transform.Find("PauseCanvas/Pause Menu");
            pauseMenu = child.GetComponent<PauseMenu>();
            fadeController = GetComponent<FadeController>();
            endingSequenceController = GetComponent<EndingSequenceController>();
        }

        public void PlayFadeIn()
        {
            
            StartCoroutine(fadeController.FadeInCorutine());
        }

        public void PlayFadeOut(float sceneLoadDelay)
        {
            StartCoroutine(fadeController.FadeOutCorutine(sceneLoadDelay));
        }

        public void PlayEndingSequence()
        {
            StartCoroutine(endingSequenceController.PlayEndingSequence());
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

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
