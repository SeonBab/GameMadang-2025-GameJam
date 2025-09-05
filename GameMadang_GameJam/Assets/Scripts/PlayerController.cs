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
    public Transform currentClimbObject;
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
        if (!currentClimbObject) return;

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

        while (currentClimbObject &&
               Mathf.Abs(currentClimbObject.InverseTransformPoint(rb.position).x) > 0.1f)
        {
            var local = currentClimbObject.InverseTransformPoint(rb.position);
            local.x = Mathf.Lerp(local.x, 0f, climbObjectSnapSpeed * Time.fixedDeltaTime);
            var next = currentClimbObject.TransformPoint(local);

            rb.MovePosition(next);
            yield return new WaitForFixedUpdate();
        }

        isClimbing = true;
    }

    internal void EndClimb()
    {
        rb.gravityScale = 1;

        isClimbing = false;

        transform.rotation = Quaternion.identity;
    }

    private void HandleClimbing()
    {
        var v = inputHandler.MoveInput.y;

        if (!currentClimbObject)
        {
            EndClimb();
            return;
        }

        Vector2 up = currentClimbObject.up;

        var targetDeg = Mathf.Atan2(up.y, up.x) * Mathf.Rad2Deg - 90f;
        var next = Mathf.LerpAngle(rb.rotation, targetDeg, Time.fixedDeltaTime * 50f);
        rb.MoveRotation(next);

        var pos = rb.position + up * (v * climbSpeed * Time.fixedDeltaTime);
        var local = currentClimbObject.InverseTransformPoint(pos);
        local.x = Mathf.Lerp(local.x, 0f, Time.fixedDeltaTime * climbObjectSnapSpeed);
        var finalPos = currentClimbObject.TransformPoint(local);
        rb.MovePosition(finalPos);

        if (v < 1f)
        {
            if (IsGround())
            {
                EndClimb();
            }
        }
    }
}