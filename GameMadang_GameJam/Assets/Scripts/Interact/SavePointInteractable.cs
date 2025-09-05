using Save;
using UnityEngine;

public class SavePointInteractable : BaseInteractable
{
    [SerializeField] private int savePointNumber = -1;
    public int SavePointNumber => savePointNumber;

    private void Awake()
    {
        if (savePointNumber == -1)
        {
            Debug.LogError(gameObject.name + " 의 savePointNumber 값 미설정");
        }
    }

    public override void Interact()
    {
        Debug.Log("세이브포인트 상호작용 시작");

        // 세이브포인트 갱신을 위해 호출하는 함수
        SaveManager.Instance.Save(this);
    }
}