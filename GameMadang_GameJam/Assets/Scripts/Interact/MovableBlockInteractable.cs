using UnityEngine;

namespace Interact
{
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

        public override void Interact(PlayerController player)
        {
            Debug.Log("이동 가능 블럭 상호작용 시작");

            if (!player || !player.CompareTag("Player"))
            {
                return;
            }

            var playerController = player.GetComponent<PlayerController>();
            if (playerController == null)
            {
                return;
            }

            playerController.OnFixedUpdateEnd += HandlePushPullRelease;
            playerController.isPushPull = true;
        }

        private void HandlePushPullRelease(float direction, GameObject movementObejct)
        {
            if (!movementObejct)
            {
                return;
            }

            var playerController = movementObejct.GetComponent<PlayerController>();
            var inputHandler = movementObejct.GetComponent<InputHandler>();
            var movementObjectrb2D = movementObejct.GetComponent<Rigidbody2D>();

            if (playerController == null || inputHandler == null || movementObjectrb2D == null)
            {
                return;
            }

            //float minX = transform.position.x - boxCollider2D.size.x * 0.5f * transform.lossyScale.x;
            //float maxX = transform.position.x + boxCollider2D.size.x * 0.5f * transform.lossyScale.x;
            float minY = transform.position.y - boxCollider2D.size.y * 0.5f * transform.lossyScale.y;
            float maxY = transform.position.y + boxCollider2D.size.y * 0.5f * transform.lossyScale.y;
            if (!playerController.IsGround() || movementObejct.transform.position.y > maxY || movementObejct.transform.position.y < minY)
            {
                return;
            }

            // 현재 오브젝트 기준 플레이어 방향(1 오른쪽, -1 왼쪽)
            float playerDir = Mathf.Sign(movementObejct.transform.position.x - transform.position.x);

            // 당기기 입력이 유효하지 않으면 구독 해제
            if ((playerDir * direction > 0) && !inputHandler.Input.Player.Interact.IsInProgress())
            {
                rb2D.linearVelocityX = 0f;
                playerController.OnFixedUpdateEnd -= HandlePushPullRelease;
                playerController.isPushPull = false;
                return;
            }

            rb2D.linearVelocity = movementObjectrb2D.linearVelocity;
        }
    }
}