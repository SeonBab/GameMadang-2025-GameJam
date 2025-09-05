using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpSlowdownRate = 0.7f;
    [SerializeField] private float climbSpeed = 10f;
    [SerializeField] private float climbObjectSnapSpeed = 30f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;

    private InputHandler inputHandler;
    private PlayerLife playerLife;
    private Rigidbody2D rb;
    private Collider2D col;

    public bool isClimbing = false;
    public Transform currentLadder;
    private float moveSpeedOrigin;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerLife = GetComponent<PlayerLife>();
        inputHandler = GetComponent<InputHandler>();
    }

    private void Start()
    {
        inputHandler.Input.Player.Move.performed += StartClimb;
        inputHandler.Input.Player.Jump.performed += JumpOnPerformed;
        inputHandler.Input.Player.Interact.performed += InteractOnPerformed;

        moveSpeedOrigin = moveSpeed;
    }

    private void FixedUpdate()
    {
        if (playerLife.IsDead) return;

        if (isClimbing)
            HandleClimbing();
        else
            HandleMovement();
    }

    private void OnDestroy()
    {
        inputHandler.Input.Player.Jump.performed -= JumpOnPerformed;
        inputHandler.Input.Player.Interact.performed -= InteractOnPerformed;
    }

    private bool IsGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
    }

    private void StartClimb(InputAction.CallbackContext ctx)
    {
        if (!currentLadder) return;

        if (ctx.ReadValue<Vector2>().y > 0f)
        {
            StartCoroutine(BeginClimbCo());
        }
    }

    private void JumpOnPerformed(InputAction.CallbackContext ctx)
    {
        if (playerLife.IsDead) return;

        if (isClimbing)
        {
            var x = inputHandler.MoveInput.x;

            var dir = Vector2.up + (Vector2.right * Mathf.Sign(x));

            print(dir.normalized);

            rb.AddForce(dir.normalized * jumpForce, ForceMode2D.Impulse);

            EndClimb();
        }
        
        if (!IsGround()) return;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void InteractOnPerformed(InputAction.CallbackContext ctx)
    {
        if (playerLife.IsDead) return;

        var interactionHandler = GetComponentInChildren<InteractionHandler>();

        if (interactionHandler)
        {
            interactionHandler.AttemptInteract();
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

        if (!IsGround())
        {
            moveSpeed = moveSpeedOrigin * jumpSlowdownRate;
        }
        else
        {
            moveSpeed = moveSpeedOrigin;
        }

        var targetVx = Mathf.Sign(x) * moveSpeed;
        rb.linearVelocity = new Vector2(targetVx, rb.linearVelocity.y);
    }

    IEnumerator BeginClimbCo()
    {
        yield return new WaitForFixedUpdate();

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        while (!Mathf.Approximately(rb.position.x, currentLadder.position.x))
        {
            var newX = Mathf.Lerp(rb.position.x, currentLadder.position.x,
                Time.fixedDeltaTime * climbObjectSnapSpeed);

            rb.MovePosition(new Vector2(newX, rb.position.y));

            yield return null;
        }

        isClimbing = true;
    }

    internal void EndClimb()
    {
        rb.gravityScale = 1;

        isClimbing = false;
    }

    private void HandleClimbing()
    {
        var v = inputHandler.MoveInput.y;

        rb.MovePosition(rb.position + Vector2.up * (v * climbSpeed * Time.fixedDeltaTime));

        if (v < 1f)
        {
            if (IsGround())
            {
                EndClimb();
            }
        }
    }
}