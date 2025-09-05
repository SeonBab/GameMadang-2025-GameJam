using UnityEngine;

public enum EInteractableType
{
    Rope,
    Ladder,
    Lever,
    MovableBlock,
    SavePoint,
}

public interface IInteract
{
    void Interact();

    bool GetIsAutoInteract();

    int GetInteractWeight();
    EInteractableType GetInteractType();
}
