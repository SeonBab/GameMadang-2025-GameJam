using UnityEngine;

public abstract class BaseInteractable : MonoBehaviour, IInteract
{
    [SerializeField] protected EInteractableType InteractableType;
    [SerializeField] protected int weight;
    [SerializeField] protected bool bIsAutoInteract;

    public abstract void Interact();

    public bool GetIsAutoInteract()
    {
        return bIsAutoInteract;
    }

    public int GetInteractWeight()
    {
        return weight;
    }

    public EInteractableType GetInteractType()
    {
        return InteractableType;
    }
}
