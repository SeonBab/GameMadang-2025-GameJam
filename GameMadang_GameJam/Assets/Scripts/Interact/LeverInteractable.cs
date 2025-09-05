using UnityEngine;

public class LeverInteractable : BaseInteractable
{
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

    public override void Interact()
    {
        Debug.Log("레버 상호작용 시작");

        isActive = !isActive;

        //타겟 오브젝트의 함수 실행
        Elevator elevator = targetObject.GetComponent<Elevator>();
        if (elevator != null)
        {
            if (isActive == true)
            {
                elevator.GoUp();
            }
            else
            {
                elevator.GoDown();
            }
        }
    }
}