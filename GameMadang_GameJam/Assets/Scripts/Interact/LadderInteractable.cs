using UnityEngine;

public class LadderInteractable : BaseInteractable
{
    public override void Interact(GameObject InteractCharacter)
    {
        Debug.Log("사다리 상호작용 시작");
    }
}