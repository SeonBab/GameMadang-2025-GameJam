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

    private void FixedUpdate()
    {
         
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
            playerController.isPushPull = false;
        }
    }

    public override void Interact(GameObject InteractCharacter)
    {
        Debug.Log("이동 가능 블럭 상호작용 시작");
        
        if (InteractCharacter == null || InteractCharacter.CompareTag("Player") == false)
        {
            return;
        }

        PlayerController playerController = InteractCharacter.GetComponent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        playerController.OnFixedUpdateEnd += HandlePushPullRelease;
        playerController.isPushPull = true;
    }

    private void HandlePushPullRelease(float Direction, GameObject MovementObejct)
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

        // 현재 오브젝트 기준 플레이어 방향(1 오른쪽, -1 왼쪽)
        float playerDir = Mathf.Sign(MovementObejct.transform.position.x - transform.position.x);

        // 당기기 입력이 유효하지 않으면 구독 해제
        if ((playerDir * Direction > 0) && !inputHandler.Input.Player.Interact.IsInProgress())
        {
            rb2D.linearVelocityX = 0f;
            playerController.OnFixedUpdateEnd -= HandlePushPullRelease;
            playerController.isPushPull = false;
            return;
        }

        // 밀려는 경우
        if (playerDir * Direction == -1)
        {
            // 밀 수 있는 거리인지 확인
            // 현재 박스 콜라이더의 X축 위치
            float minX = transform.position.x - boxCollider2D.size.x * 0.5f * transform.lossyScale.x;
            float maxX = transform.position.x + boxCollider2D.size.x * 0.5f * transform.lossyScale.x;
            // 현재 캐릭터 상호작용 콜라이더의 X축 위치
            float PlayerminX = transform.position.x - capsuleCollider2D.size.x * 0.5f * transform.lossyScale.x;
            float PlayermaxX = transform.position.x + capsuleCollider2D.size.x * 0.5f * transform.lossyScale.x;
            // 겹치는 범위 계산
            float overlapMin = Mathf.Max(minX, PlayerminX);
            float overlapMax = Mathf.Min(maxX, PlayermaxX);
            float overlap = Mathf.Max(0f, overlapMax - overlapMin);
            // 겹치는 비율 계산
            float interactionPercent = (overlap / interactionHandler.InteractionDistanceX) * 100f;
            // 필요한 만큼 겹치지 않았다면 밀지 않고 리턴
            if (interactionPercent < pushDisntanceFactor)
            {
                return;
            }
        }

        // 밀거나 당길 수 있는 높이인지 확인
        float minY = transform.position.y - boxCollider2D.size.y * 0.5f * transform.lossyScale.y;
        float maxY = transform.position.y + boxCollider2D.size.y * 0.5f * transform.lossyScale.y;
        if (!playerController.IsGround() || MovementObejct.transform.position.y > maxY || MovementObejct.transform.position.y < minY)
        {
            return;
        }

        rb2D.linearVelocity = movementObjectrb2D.linearVelocity;
    }
}