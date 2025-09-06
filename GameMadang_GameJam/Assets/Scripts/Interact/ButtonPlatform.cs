using Save;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPlatform : MonoBehaviour
{
    BoxCollider2D boxCollider2D;

    private HashSet<GameObject> objectsInTrigger = new HashSet<GameObject>();

    // 레버 동작의 대상이 되는 오브젝트
    [SerializeField] private GameObject targetObject;
    private bool isActive = false;

    private void Awake()
    {
        if (targetObject == null)
        {
            Debug.LogError(gameObject.name + " 의 동작 대상 미설정");
        }
        else if (targetObject.GetComponent<Elevator>() == null)
        {
            Debug.LogError(gameObject.name + " 의 잘못된 동작 대상 설정");
        }
    }

    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectsInTrigger.Add(collision.gameObject);
        if (!isActive)
        {
            isActive = true;

            // targetObject의 동작 함수 실행
            Debug.Log("발판 버튼 상호작용 시작");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objectsInTrigger.Remove(collision.gameObject);
        if (isActive && objectsInTrigger.Count == 0)
        {
            isActive = false;

            // targetObject의 동작 중지
            Debug.Log("발판 버튼 상호작용 시작");
        }
    }
}
