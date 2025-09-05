using UnityEngine;

namespace Save
{
    public class SaveManager : MonoBehaviour
    {
        public SavePoint currentSavePoint;
        public static SaveManager Instance { get; private set; }

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
            var savePoint = PlayerPrefs.GetInt("SavePoint", 0);

            foreach (Transform child in transform)
            {
                var point = child.GetComponent<SavePoint>();
                if (point.number != savePoint) continue;

                currentSavePoint = point;
                return;
            }
        }

        public SavePoint GetCurrentSavePoint()
        {
            return currentSavePoint;
        }

        public void Save(SavePoint savePoint)
        {
            if (currentSavePoint.number < savePoint.number)
            {
                currentSavePoint = savePoint;
                PlayerPrefs.SetInt("SavePoint", currentSavePoint.number);
            }
        }

        public void Load(Transform player)
        {
            player.position = currentSavePoint.transform.position;
        }
    }
}