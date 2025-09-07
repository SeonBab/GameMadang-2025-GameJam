using System;
using Interact;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static readonly int IsMove = Animator.StringToHash("IsMove");
    private static readonly int IsJump = Animator.StringToHash("IsJump");
    private static readonly int ReadyPushPull = Animator.StringToHash("ReadyPushPull");
    private static readonly int IsPull = Animator.StringToHash("IsPull");
    private static readonly int IsPush = Animator.StringToHash("IsPush");

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpSlowdownRate = 0.7f;
    [SerializeField] private float interactionForce = 0.5f;

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
    private Animator animator;

    public bool isReadyPushPull;
    public bool isPull;
    public bool isPush;

    private float moveSpeedOrigin;

    public Action<Vector2, GameObject> OnFixedUpdateEnd;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        parkour = GetComponent<Parkour>();
        playerLife = GetComponent<PlayerLife>();
        playerClimb = GetComponent<PlayerClimb>();
        inputHandler = GetComponent<InputHandler>();
        interactionHandler = GetComponentInChildren<InteractionHandler>();
    }

    private void Start()
    {
        inputHandler.Input.Player.Jump.performed += JumpOnPerformed;
        inputHandler.Input.Player.Interact.performed += InteractOnPerformed;

        moveSpeedOrigin = moveSpeed;

        InputHandler.OnRemoveInputCallbacks += RemoveInputCallbacks;
    }

    private void OnDestroy()
    {
        RemoveInputCallbacks();
        InputHandler.OnRemoveInputCallbacks -= RemoveInputCallbacks;
    }

    private void Update()
    {
        if (playerLife.IsDead) return;
        if (parkour.IsBusy) return;
        if (IsGround()) return;

        var hit = parkour.IsParkour();
        if (hit)
        {
            parkour.StartParkour(hit);
        }
    }

    private void FixedUpdate()
    {
        if (playerLife.IsDead) return;
        if (parkour.IsBusy) return;

        HandleMovement();

        OnFixedUpdateEnd?.Invoke(inputHandler.MoveInput, gameObject);
    }

    private void LateUpdate()
    {
        FlipSprite();

        animator.SetBool(IsJump, !IsGround() && !playerClimb.IsClimbing && !parkour.IsBusy && !playerLife.IsDead);
    }

    public bool IsGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
    }

    private void FlipSprite()
    {
        if (playerLife.IsDead) return;
        if(isReadyPushPull) return;
        if (parkour.IsBusy) return;

        sr.flipX = inputHandler.MoveInput.x switch
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
        if (!parkour.CanJump) return;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void InteractOnPerformed(InputAction.CallbackContext ctx)
    {
        if (playerLife.IsDead) return;

        interactionHandler.TryInteract();
    }

    private void HandleMovement()
    {
        if(playerClimb.IsClimbing) return;

        var x = inputHandler.MoveInput.x;

        if (Mathf.Abs(x) < 0.1f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            animator.SetBool(IsMove, false);
            animator.SetBool(IsPull, isPull);
            animator.SetBool(IsPush, isPush);

            return;
        }

        if (!IsGround())
        {
            moveSpeed = moveSpeedOrigin * jumpSlowdownRate;
        }
        else
        {
            moveSpeed = moveSpeedOrigin;

            if (isPush || isPull)
            {
                moveSpeed = moveSpeedOrigin * interactionForce;
            }
        }

        var targetVx = Mathf.Sign(x) * moveSpeed;
        rb.linearVelocity = new Vector2(targetVx, rb.linearVelocity.y);

        animator.SetBool(IsMove, true);
        animator.SetBool(IsPull, isPull);
        animator.SetBool(IsPush, isPush);
    }

    public void RemoveInputCallbacks()
    {
        inputHandler.Input.Player.Jump.performed -= JumpOnPerformed;
        inputHandler.Input.Player.Interact.performed -= InteractOnPerformed;
    }
}