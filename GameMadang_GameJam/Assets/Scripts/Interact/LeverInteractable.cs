using UnityEngine;

public class LeverInteractable : BaseInteractable
{
    // 레버 동작의 대상이 되는 오브젝트
    [SerializeField] private GameObject targetObject;

    public override void Interact()
    {
        Debug.Log("레버 상호작용 시작");

        //TODO
        //타겟 오브젝트의 함수 실행
    }
}