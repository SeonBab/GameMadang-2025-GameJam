using UnityEngine;
using UnityEngine.Serialization;

namespace Interact
{
    public abstract class BaseInteractable : MonoBehaviour // , IInteract
    {
        public enum EInteractableType
        {
            None,
            Rope,
            Ladder,
            Lever,
            MovableBlock,
            SavePoint
        }

        [SerializeField] protected EInteractableType interactableType;
        [SerializeField] protected int weight;
        [SerializeField] protected bool bIsAutoInteract;

        private Collider2D col;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        public bool IsAuto => bIsAutoInteract;
        public int Weight => weight;
        public EInteractableType Type => interactableType;
        public virtual float GetBottom => col.bounds.min.y;
        public virtual float GetTop => col.bounds.max.y;

        public abstract void Interact(PlayerController player);
    }
}