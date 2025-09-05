using UnityEngine;

namespace Save
{
    public class SaveManager : MonoBehaviour
    {
        public SavePointInteractable currentSavePoint;

        public static SaveManager Instance { get; private set; }

        private Transform player;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;

            var savePoint = PlayerPrefs.GetInt("SavePoint", 0);

            foreach (Transform child in transform)
            {
                var point = child.GetComponent<SavePointInteractable>();
                if (point.SavePointNumber != savePoint) continue;

                currentSavePoint = point;
                return;
            }
        }

        public SavePointInteractable GetCurrentSavePoint()
        {
            return currentSavePoint;
        }

        public void Save(SavePointInteractable savePoint)
        {
            if (currentSavePoint.SavePointNumber < savePoint.SavePointNumber)
            {
                currentSavePoint = savePoint;
                PlayerPrefs.SetInt("SavePoint", currentSavePoint.SavePointNumber);
            }
        }

        public void Load()
        {
            if (!currentSavePoint) return;

            player.position = currentSavePoint.transform.position;
        }
    }
}