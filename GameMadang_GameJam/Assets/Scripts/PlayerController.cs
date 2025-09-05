using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpSpeed = 5f;
    private Collider2D col;

    private InputHandler inputHandler;
    private PlayerLife playerLife;
    private Rigidbody2D rb;
    private PlayerCharacter playerCharacter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerLife = GetComponent<PlayerLife>();
        inputHandler = GetComponent<InputHandler>();
    }

    private void Start()
    {
        inputHandler.Input.Player.Jump.performed += JumpOnPerformed;
        inputHandler.Input.Player.Interact.performed += InteractOnPerformed;
    }

    private void FixedUpdate()
    {
        if (playerLife.IsDead) return;

        HandleMovement();
    }

    private void OnDestroy()
    {
        inputHandler.Input.Player.Jump.performed -= JumpOnPerformed;
        inputHandler.Input.Player.Interact.performed -= InteractOnPerformed;
    }

    private void JumpOnPerformed(InputAction.CallbackContext ctx)
    {
        if (playerLife.IsDead) return;

        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
    }

    private void InteractOnPerformed(InputAction.CallbackContext ctx)
    {
        if (playerLife.IsDead) return;

        if (playerCharacter)
        {
            playerCharacter.AttemptInteract();
        }
    }

    private void HandleMovement()
    {
        var x = inputHandler.MoveInput.x;

        if (Mathf.Abs(x) < 0.1f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        var targetVx = Mathf.Sign(x) * moveSpeed;
        rb.linearVelocity = new Vector2(targetVx, rb.linearVelocity.y);
    }
}