using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class InteractionHandler : MonoBehaviour
{
    [SerializeField] LayerMask interactTargetLayer;
    CapsuleCollider2D interactCollider;

    private void Awake()
    {
        interactCollider = GetComponent<CapsuleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AttemptAutoInteract(collision);
    }

    #region Interact

    // �ڵ����� ��ȣ�ۿ��� �õ��ϴ� �Լ�
    private void AttemptAutoInteract(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }
        // #1 ��ȣ�ۿ�
        // ��ȣ�ۿ� ������� Ȯ��
        IInteract InteractTarget = collision.GetComponent<IInteract>();
        if (InteractTarget == null)
        {
            return;
        }

        // #2 ��ȣ�ۿ�
        // ��ȣ�ۿ��� �ڵ����� �̷������ �ϴ��� Ȯ�� �� ����
        bool bIsAutoInteract = InteractTarget.GetIsAutoInteract();
        if (bIsAutoInteract)
        {
            // #3 ��ȣ�ۿ�
            // ��ȣ�ۿ� ȣ��
            InteractTarget.Interact();
        }
    }

    // �Ϲ����� ��ȣ�ۿ��� �õ��ϴ� �Լ�
    public void AttemptInteract()
    {
        if (interactCollider)
        {
            float xRadius = interactCollider.size.x;
            float yRadius = interactCollider.size.x;

            // ��ȣ�ۿ��� ������ �Ÿ��� �׷��ִ� ����� ����
            Debug.DrawLine(transform.position, transform.position + Vector3.up * yRadius, Color.green, 1f);
            Debug.DrawLine(transform.position, transform.position + Vector3.down * yRadius, Color.green, 1f);
            Debug.DrawLine(transform.position, transform.position + Vector3.left * xRadius, Color.green, 1f);
            Debug.DrawLine(transform.position, transform.position + Vector3.right * xRadius, Color.green, 1f);

            Vector2 capsuleSize = new Vector2(xRadius, yRadius);
            Collider2D[] hits = Physics2D.OverlapCapsuleAll(transform.position, capsuleSize, CapsuleDirection2D.Vertical, 0f, interactTargetLayer);

            int maxWeight = 0;
            Collider2D interactTarget = null;

            // ��ȣ�ۿ� ����� ã�ƿ´�.
            foreach (var hit in hits)
            {
                var interactable = hit.GetComponent<IInteract>();
                if (interactable != null)
                {
                    int currentWeight = interactable.GetInteractWeight();

                    if (currentWeight > maxWeight)
                    {
                        // ����ġ�� ���� ���� ������Ʈ�� ����
                        maxWeight = currentWeight;
                        interactTarget = hit;
                    }
                    else if (currentWeight == maxWeight)
                    {
                        // �Ÿ��� ���� ����� ������Ʈ�� ����
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
