using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MovableBlockInteractable : BaseInteractable
{
    private Rigidbody2D rb2D;
    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        playerController.OnGroundMovement -= HandlePushPullRelease;
        playerController.isPushPull = false;
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

        playerController.OnGroundMovement += HandlePushPullRelease;
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

        // 현재 오브젝트 기준 플레이어 방향(1 오른쪽, -1 왼쪽)
        float playerDir = Mathf.Sign(MovementObejct.transform.position.x - transform.position.x);

        // 당기기 입력이 유효하지 않으면 구독 해제
        if ((playerDir * Direction > 0) && !inputHandler.Input.Player.Interact.IsInProgress())
        {
            playerController.OnGroundMovement -= HandlePushPullRelease;
            playerController.isPushPull = false;
            return;
        }

        Vector2 worldSize = Vector2.Scale(boxCollider2D.size, transform.lossyScale);

        float minY = transform.position.y - boxCollider2D.size.y * 0.5f * transform.lossyScale.y;
        float maxY = transform.position.y + boxCollider2D.size.y * 0.5f * transform.lossyScale.y;
        if (!playerController.IsGround() || MovementObejct.transform.position.y > maxY || MovementObejct.transform.position.y < minY)
        {
            return;
        }

        rb2D.linearVelocityX = movementObjectrb2D.linearVelocityX;
    }
}