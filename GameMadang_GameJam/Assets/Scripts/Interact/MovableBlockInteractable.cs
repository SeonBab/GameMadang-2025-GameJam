using UnityEngine;

public class MovableBlockInteractable : BaseInteractable
{
    public override void Interact()
    {
        Debug.Log("이동 가능 블럭 상호작용 시작");
    }
}