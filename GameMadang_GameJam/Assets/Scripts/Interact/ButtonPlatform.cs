using System.Collections.Generic;
using UnityEngine;

namespace Interact
{
    public class ButtonPlatform : MonoBehaviour
    {
        [SerializeField] private List<GameObject> targetObjects = new();
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite pushedSprite;

        private readonly HashSet<GameObject> objectsInTrigger = new();
        private SpriteRenderer sr;

        // 버튼 발판 동작의 대상이 되는 오브젝트
        private bool isActive;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();

            foreach (var obj in targetObjects)
            {
                if (obj == null)
                {
                    Debug.LogError(gameObject.name + " 의 동작 대상 미설정");
                }
                else if (obj.GetComponent<ISwitch>() == null)
                {
                    Debug.LogError(gameObject.name + " 의 잘못된 동작 대상 설정");
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            objectsInTrigger.Add(collision.gameObject);
            if (isActive) return;
            isActive = true;

            Debug.Log("발판 버튼 상호작용 시작");

            sr.sprite = pushedSprite;

            //타겟 오브젝트들의 함수 실행
            foreach (var obj in targetObjects)
            {
                var targetSwitch = obj.GetComponent<ISwitch>();
                targetSwitch?.OnSwitch();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            objectsInTrigger.Remove(collision.gameObject);
            if (!isActive || objectsInTrigger.Count != 0) return;
            isActive = false;

            Debug.Log("발판 버튼 상호작용 해제");

            sr.sprite = defaultSprite;

            //타겟 오브젝트들의 함수 실행
            foreach (var obj in targetObjects)
            {
                var targetSwitch = obj.GetComponent<ISwitch>();
                targetSwitch?.OffSwitch();
            }
        }
    }
}
