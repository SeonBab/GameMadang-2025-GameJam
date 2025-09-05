using UnityEngine;

public class RopeInteractable : BaseInteractable
{
    public override void Interact()
    {
        Debug.Log("로프 상호작용 시작");

        // 플레이어가 로프의 중심으로 이동되어야한다.
    }
}