using Save;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPlatform : MonoBehaviour
{
    BoxCollider2D boxCollider2D;

    private HashSet<GameObject> objectsInTrigger = new HashSet<GameObject>();

    // 버튼 발판 동작의 대상이 되는 오브젝트
    [SerializeField] private List<GameObject> targetObjects = new List<GameObject>();
    private bool isActive = false;

    private void Awake()
    {
        foreach (GameObject obj in targetObjects)
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

            Debug.Log("발판 버튼 상호작용 시작");

            //타겟 오브젝트들의 함수 실행
            foreach (GameObject obj in targetObjects)
            {
                ISwitch targetSwitch = obj.GetComponent<ISwitch>();
                if (targetSwitch != null)
                {
                    targetSwitch.OnSwitch();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objectsInTrigger.Remove(collision.gameObject);
        if (isActive && objectsInTrigger.Count == 0)
        {
            isActive = false;

            Debug.Log("발판 버튼 상호작용 해제");

            //타겟 오브젝트들의 함수 실행
            foreach (GameObject obj in targetObjects)
            {
                ISwitch targetSwitch = obj.GetComponent<ISwitch>();
                if (targetSwitch != null)
                {
                    targetSwitch.OffSwitch();
                }
            }
        }
    }
}
