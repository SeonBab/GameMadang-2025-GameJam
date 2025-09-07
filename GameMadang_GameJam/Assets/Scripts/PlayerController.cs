using System;
using Interact;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpSlowdownRate = 0.7f;
    [SerializeField] private float climbSpeed = 10f;
    public float ClimbSpeed => climbSpeed;
    [SerializeField] private float climbObjectSnapSpeed = 30f;
    [SerializeField] private float interactionForce = 0.5f;
    public float InteractionForce => interactionForce;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;

    private InputHandler inputHandler;
    private InteractionHandler interactionHandler;
    private PlayerClimb playerClimb;
    private PlayerLife playerLife;
    private Parkour parkour;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;

    public bool isPushPull;

    private float moveSpeedOrigin;

    public Action<Vector2, GameObject> OnFixedUpdateEnd;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        parkour = GetComponent<Parkour>();
        playerLife = GetComponent<PlayerLife>();
        playerClimb = GetComponent<PlayerClimb>();
        inputHandler = GetComponent<InputHandler>();
        interactionHandler = GetComponentInChildren<InteractionHandler>();
    }

    private void Start()
    {
        inputHandler.Input.Player.Move.performed += FlipSprite;
        inputHandler.Input.Player.Jump.performed += JumpOnPerformed;
        inputHandler.Input.Player.Interact.performed += InteractOnPerformed;

        moveSpeedOrigin = moveSpeed;
    }

    private void OnDestroy()
    {
        inputHandler.Input.Player.Move.performed -= FlipSprite;
        inputHandler.Input.Player.Jump.performed -= JumpOnPerformed;
        inputHandler.Input.Player.Interact.performed -= InteractOnPerformed;
    }

    private void Update()
    {
        if (playerLife.IsDead) return;
        if (parkour.IsBusy()) return;

        var hit = parkour.IsParkour();
        if (hit)
        {
            parkour.StartParkour(hit);
        }
    }

    private void FixedUpdate()
    {
        if (playerLife.IsDead) return;
        if (parkour.IsBusy()) return;

        HandleMovement();

        OnFixedUpdateEnd?.Invoke(inputHandler.MoveInput, gameObject);
    }

    public bool IsGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
    }

    private void FlipSprite(InputAction.CallbackContext ctx)
    {
        if (playerLife.IsDead) return;
        if (playerClimb.IsClimbing) return;

        sr.flipX = ctx.ReadValue<Vector2>().x switch
        {
            > 0 => true,
            < 0 => false,
            _ => sr.flipX
        };
    }

    private void JumpOnPerformed(InputAction.CallbackContext ctx)
    {
        if (playerLife.IsDead) return;
        if (playerClimb.IsClimbing) return;
        if (!IsGround()) return;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void InteractOnPerformed(InputAction.CallbackContext ctx)
    {
        if (playerLife.IsDead) return;

        interactionHandler.TryZInteract();
    }

    private void HandleMovement()
    {
        if(playerClimb.IsClimbing) return;

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

            if (isPushPull)
            {
                moveSpeed = moveSpeedOrigin * InteractionForce;
            }
        }

        var targetVx = Mathf.Sign(x) * moveSpeed;
        rb.linearVelocity = new Vector2(targetVx, rb.linearVelocity.y);
    }
}