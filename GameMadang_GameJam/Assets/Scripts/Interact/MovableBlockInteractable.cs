using Interact;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MovableBlockInteractable : BaseInteractable
{
    [SerializeField, Range(0, 100)] float pushDisntanceFactor = 90f;

    private Rigidbody2D rb2D;
    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject playerGameObject = collision.transform.parent.gameObject;
            PlayerController playerController = playerGameObject.GetComponent<PlayerController>();
            if (playerController == null)
            {
                return;
            }

            rb2D.linearVelocityX = 0f;
            playerController.OnFixedUpdateEnd -= HandlePushPullRelease;
            playerController.isPush = false;
            playerController.isPull = false;
            playerController.isReadyPushPull = false;
        }
    }

    public override void Interact(PlayerController player)
    {
        Debug.Log("이동 가능 블럭 상호작용 시작");
        
        player.OnFixedUpdateEnd += HandlePushPullRelease;
        player.isPush = false;
        player.isPull = false;
        player.isReadyPushPull = true;
    }

    private void HandlePushPullRelease(Vector2 Direction, GameObject MovementObejct)
    {
        if (MovementObejct == null)
        {
            return;
        }

        PlayerController playerController = MovementObejct.GetComponent<PlayerController>();
        InputHandler inputHandler = MovementObejct.GetComponent<InputHandler>();
        Rigidbody2D movementObjectrb2D = MovementObejct.GetComponent<Rigidbody2D>();
        if (playerController == null || inputHandler == null || movementObjectrb2D == null)
        {
            return;
        }

        Transform childTransform = MovementObejct.transform.Find("Interact");
        CapsuleCollider2D capsuleCollider2D = childTransform.GetComponentInChildren<CapsuleCollider2D>();
        InteractionHandler interactionHandler = childTransform.GetComponentInChildren<InteractionHandler>();
        if (capsuleCollider2D == null || interactionHandler == null)
        {
            return;
        }

        // 캐릭터가 공중에 떠있거나 유효하지 않은 높이의 경우 블럭이 움직이지 않도록 한다.
        float minY = boxCollider2D.bounds.min.y;
        float maxY = boxCollider2D.bounds.max.y;
        if (Direction.y > 0f || !playerController.IsGround() || MovementObejct.transform.position.y > maxY || MovementObejct.transform.position.y < minY)
        {
            playerController.isPush = false;
            playerController.isPull = false;
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }
        else
        {
            rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // 현재 오브젝트 기준 플레이어 방향(1 오른쪽, -1 왼쪽)
        float playerDir = Mathf.Sign(MovementObejct.transform.position.x - transform.position.x);

        // 당기기 입력이 유효하지 않으면 구독 해제
        if ((playerDir * Direction.x > 0) && !inputHandler.Input.Player.Interact.IsInProgress())
        {
            rb2D.linearVelocityX = 0f;
            playerController.OnFixedUpdateEnd -= HandlePushPullRelease;
            playerController.isPush = false;
            playerController.isPull = false;
            return;
        }

        // 밀 수 있는지 확인
        if (playerDir * Direction.x == -1)
        {
            // 밀 수 있는 거리인지 확인
            // 현재 박스 콜라이더의 X축 위치
            float minX = boxCollider2D.bounds.min.x;
            float maxX = boxCollider2D.bounds.max.x;
            // 현재 캐릭터 상호작용 콜라이더의 X축 위치
            float PlayerminX = capsuleCollider2D.bounds.min.x;
            float PlayermaxX = capsuleCollider2D.bounds.max.x;
            // 겹치는 범위 계산
            float overlapMin = Mathf.Max(minX, PlayerminX);
            float overlapMax = Mathf.Min(maxX, PlayermaxX);
            float overlap = Mathf.Max(0f, overlapMax - overlapMin);
            // 겹치는 비율 계산
            float interactionPercent = (overlap / interactionHandler.ColliderEdgeDistanceX) * 100f;
            // 필요한 만큼 겹치지 않았다면 밀지 않고 리턴
            if (interactionPercent < pushDisntanceFactor)
            {
                return;
            }
        }

        // 상태 값 변경
        if (playerDir * Direction.x == -1)
        {
            playerController.isPush = true;
            playerController.isPull = false;
        }
        else if (playerDir * Direction.x == 1)
        {
            playerController.isPush = false;
            playerController.isPull = true;
        }
        else
        {
            playerController.isPush = false;
            playerController.isPull = false;;
        }

        rb2D.linearVelocity = movementObjectrb2D.linearVelocity;
    }
}