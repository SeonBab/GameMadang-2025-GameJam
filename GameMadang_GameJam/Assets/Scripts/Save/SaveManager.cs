using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

namespace Save
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [SerializeField] private GameObject savePointPrefab;

        [SerializeField] private List<SavePointInteractable> savePoints = new List<SavePointInteractable>();

        [SerializeField] public SavePointInteractable currentSavePoint;

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

            StartCoroutine(OnPreRender());
        }

        // 모든 Start함수가 호출 된 뒤 플레이어 캐릭터 위치 로드
        private IEnumerator OnPreRender()
        {
            yield return new WaitForEndOfFrame();

            Load();
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
                PlayerPrefs.SetInt("SavePointIndex", currentSavePoint.SavePointNumber);
                PlayerPrefs.Save();
            }
        }

        public void Load()
        {
            int savePointIndex = PlayerPrefs.GetInt("SavePointIndex", 0);

            if (savePoints[savePointIndex] == null)
            {
                currentSavePoint = savePoints[0];
            }
            else
            {
                currentSavePoint = savePoints[savePointIndex];
            }

            if (currentSavePoint == null) return;

            player.position = currentSavePoint.transform.position;
        }

        [ContextMenu("Generate SavePoint")]
        public void GenerateSavePoint()
        {
            GameObject newSavePoint = Instantiate(savePointPrefab, transform);

            newSavePoint.name = "SavePoint_" + savePoints.Count;

            Vector3 spawnposition = transform.position;
            spawnposition.y += 3f;
            newSavePoint.transform.position = spawnposition;

            SavePointInteractable savePointInteractable = newSavePoint.GetComponent<SavePointInteractable>();
            savePointInteractable.SavePointNumber = savePoints.Count;

            savePoints.Add(savePointInteractable);

            if (savePoints.Count == 1)
            {
                currentSavePoint = savePointInteractable;
            }
        }
    }
}