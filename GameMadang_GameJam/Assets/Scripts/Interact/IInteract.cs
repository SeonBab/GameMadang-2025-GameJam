public enum EInteractableType
{
    None,
    Rope,
    Ladder,
    Lever,
    MovableBlock,
    SavePoint
}

public interface IInteract
{
    void Interact();

    bool GetIsAutoInteract();

    int GetInteractWeight();

    EInteractableType GetInteractType();
}