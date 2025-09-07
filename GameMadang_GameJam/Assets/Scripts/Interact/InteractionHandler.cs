using System.Collections.Generic;
using UnityEngine;

namespace Interact
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class InteractionHandler : MonoBehaviour
    {
        [SerializeField] LayerMask interactLayer;

        private PlayerController player;
        private ContactFilter2D filter;
        private CapsuleCollider2D interactCollider;

        public float CenterDistanceX { get; private set; }

        public float ColliderEdgeDistanceX { get; private set; }

        private void Awake()
        {
            player = GetComponentInParent<PlayerController>();
            interactCollider = GetComponent<CapsuleCollider2D>();

            filter.SetLayerMask(interactLayer);
        }

        private void Start()
        {
            var capsuleCollider2D = transform.parent.GetComponent<CapsuleCollider2D>();

            var playerCenterX = transform.parent.position.x;
            var interactMaxX = interactCollider.bounds.max.x;
            CenterDistanceX = Mathf.Abs(interactMaxX - playerCenterX);

            var interactionMinX = transform.position.x - interactCollider.size.x * 0.5f * transform.lossyScale.x;
            var playerCapsuleMinX = transform.position.x + capsuleCollider2D.size.x * 0.5f * transform.lossyScale.x;

            ColliderEdgeDistanceX = Mathf.Abs(interactionMinX - playerCapsuleMinX);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.layer.Equals(LayerMask.NameToLayer("Interact"))) return;

            TryAutoInteract(other);
        }

        // 자동으로 상호작용을 시도하는 함수
        private void TryAutoInteract(Collider2D other)
        {
            if (!other) return;

            // #1 상호작용
            // 상호작용 대상인지 확인
            if (!other.TryGetComponent<BaseInteractable>(out var interactTarget)) return;

            // #2 상호작용
            // 상호작용이 자동으로 이루어져야 하는지 확인 및 실행
            if (!interactTarget.IsAuto) return;

            // #3 상호작용
            // 상호작용 호출
            interactTarget.Interact(player);
        }

        public void TryInteract()
        {
            if (!interactCollider) return;

            List<Collider2D> hits = new();
            Physics2D.OverlapCapsule(transform.position, interactCollider.bounds.size,
                CapsuleDirection2D.Vertical, 0f, filter, hits);

            if (hits.Count <= 0) return;

            var origin = transform.position;
            hits.Sort((a, b) =>
            {
                a.TryGetComponent<BaseInteractable>(out var ai);
                b.TryGetComponent<BaseInteractable>(out var bi);

                if (ai == null || bi == null) return (bi != null).CompareTo(ai != null);

                var byWeight = bi.Weight.CompareTo(ai.Weight);
                if (byWeight != 0) return byWeight;

                var da = Mathf.Abs(a.transform.position.x - origin.x);
                var db = Mathf.Abs(b.transform.position.x - origin.x);

                return da.CompareTo(db);
            });

            hits[0].GetComponent<BaseInteractable>().Interact(player);
        }
    }
}