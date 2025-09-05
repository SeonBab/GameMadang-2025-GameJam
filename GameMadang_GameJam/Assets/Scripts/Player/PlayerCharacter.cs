using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] LayerMask interactTargetLayer;
    CircleCollider2D interactCollider;

    private void Awake()
    {
        interactCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AttemptAutoInteract(collision);
    }

    #region Interact

    // 자동으로 상호작용을 시도하는 함수
    private void AttemptAutoInteract(Collider2D collision)
    {
        if (collision == null)
        {

        }
        // #1 상호작용
        // 상호작용 대상인지 확인
        IInteract InteractTarget = collision.GetComponent<IInteract>();
        if (InteractTarget == null)
        {
            return;
        }

        // #2 상호작용
        // 상호작용이 자동으로 이루어져야 하는지 확인 및 실행
        bool bIsAutoInteract = InteractTarget.GetIsAutoInteract();
        if (bIsAutoInteract)
        {
            // #3 상호작용
            // 상호작용 호출
            InteractTarget.Interact();
        }
    }

    // 일반적인 상호작용을 시도하는 함수
    public void AttemptInteract()
    {
        if (interactCollider)
        {
            float radius = interactCollider.radius;

            // 상호작용이 가능한 거리를 그려주는 디버그 라인
            Debug.DrawLine(transform.position, transform.position + Vector3.up * radius, Color.green, 0.1f);
            Debug.DrawLine(transform.position, transform.position + Vector3.left * radius, Color.green, 0.1f);
            Debug.DrawLine(transform.position, transform.position + Vector3.right * radius, Color.green, 0.1f);
            Debug.DrawLine(transform.position, transform.position + Vector3.down * radius, Color.green, 0.1f);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, interactTargetLayer);

            int maxWeight = 0;
            Collider2D interactTarget = null;

            // 상호작용 대상을 찾아온다.
            foreach (var hit in hits)
            {
                var interactable = hit.GetComponent<IInteract>();
                if (interactable != null)
                {
                    int currentWeight = interactable.GetInteractWeight();

                    if (currentWeight > maxWeight)
                    {
                        // 가중치가 가장 높은 오브젝트로 설정
                        maxWeight = currentWeight;
                        interactTarget = hit;
                    }
                    else if (currentWeight == maxWeight)
                    {
                        // 거리가 가장 가까운 오브젝트로 설정
                        float interactTargetDistance = Vector3.Distance(transform.position, interactTarget.transform.position);
                        float hitDistance = Vector3.Distance(transform.position, hit.transform.position);

                        if (interactTargetDistance > hitDistance)
                        {
                            interactTarget = hit;
                        }
                    }
                }
            }

            if (interactTarget != null)
            {
                IInteract interactable = interactTarget.GetComponent<IInteract>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }

    #endregion
}
