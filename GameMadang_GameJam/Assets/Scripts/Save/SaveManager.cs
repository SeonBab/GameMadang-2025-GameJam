using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Save
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance { get; private set; }

        [SerializeField] private GameObject savePointPrefab;

        [SerializeField] private List<SavePointInteractable> savePoints = new List<SavePointInteractable>();

        [SerializeField] public SavePointInteractable currentSavePoint;

        private Transform player;

        [ContextMenuItem("PlayerCharacter To SpawnPoint", "MovePlayerToSavePoint")]
        [Header("Debug")]
        public int playerMoveTargetIndex;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeSaveManager(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            SceneManager.sceneLoaded += InitializeSaveManager;
        }

        private void InitializeSaveManager(Scene scene, LoadSceneMode mode)
        {
            player = GameObject.Find("Player").transform;
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
            if (savePoints.Count == 0)
            {
                return;
            }

            int savePointIndex = PlayerPrefs.GetInt("SavePointIndex", 0);
            
            if (savePoints[savePointIndex] == null)
            {
                if (savePoints[0] == null)
                {
                    return;
                }

                currentSavePoint = savePoints[0];
            }
            else
            {
                currentSavePoint = savePoints[savePointIndex];
            }

            if (currentSavePoint == null) return;

            StartCoroutine(SetPlayerPosition());
            IEnumerator SetPlayerPosition()
            {
                yield return new WaitForFixedUpdate();

                player.position = currentSavePoint.transform.position;
            }
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

        [ContextMenu("Debug/MovePlayer To SavePoint")]
        public void MovePlayerToSavePoint()
        {
            SavePointInteractable targetSavePoint = savePoints[playerMoveTargetIndex];
            if (targetSavePoint == null)
            {
                Debug.LogError("잘못된 인덱스입니다.");
                return;
            }

            StartCoroutine(SetPlayerPosition());
            IEnumerator SetPlayerPosition()
            {
                yield return new WaitForFixedUpdate();

                player.position = targetSavePoint.transform.position;
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