using UnityEngine;

public class RopeInteractable : BaseInteractable
{
    [SerializeField, Range(0f, 0.1f)] private float forceMultiplier = 0.05f;
    [SerializeField, Range(0f, 10f)] private float maxXSpeed = 1f;
    [SerializeField, Range(0f, 5f)] private float maxYSpeed = 0.5f;

    private Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
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

            playerController.OnFixedUpdateEnd -= RopeAddForce;
            if (playerController.currentClimbObject == gameObject.transform)
            {
                playerController.currentClimbObject = null;
            }
        }
    }

    public override void Interact(GameObject InteractCharacter)
    {
        Debug.Log("로프 상호작용 시작");

        if (InteractCharacter == null || InteractCharacter.CompareTag("Player") == false)
        {
            return;
        }

        PlayerController playerController = InteractCharacter.GetComponent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        playerController.OnFixedUpdateEnd += RopeAddForce;
        playerController.currentClimbObject = gameObject.transform;
    }

    private void RopeAddForce(Vector2 Direction, GameObject MovementObejct)
    {
        Vector2 forceToAdd = new Vector2(Mathf.Sign(Direction.x) * forceMultiplier, 0f);
        rb2D.AddForce(forceToAdd, ForceMode2D.Impulse);

        Vector2 currentVelocity = rb2D.linearVelocity;

        // X축 속도 제한
        if (Mathf.Abs(currentVelocity.x) > maxXSpeed)
        {
            currentVelocity.x = Mathf.Sign(currentVelocity.x) * maxXSpeed;
        }

        // Y축 속도 제한
        if (Mathf.Abs(currentVelocity.x) > maxXSpeed)
        {
            currentVelocity.y = Mathf.Sign(currentVelocity.y) * maxYSpeed;
        }

        // 속도 제한 적용
        rb2D.linearVelocity = currentVelocity;
    }
}