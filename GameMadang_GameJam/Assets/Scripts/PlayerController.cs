using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    private InputHandler inputHandler;
    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        inputHandler = GetComponent<InputHandler>();

        inputHandler.Input.Player.Jump.performed += JumpOnPerformed;
    }

    private void JumpOnPerformed(InputAction.CallbackContext obj)
    {
        Debug.Log("Jump");
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        var moveInput = inputHandler.MoveInput;

        if (moveInput == Vector2.zero) return;

        var moveDirection = Vector2.right * moveInput.x;

        rb.MovePosition(rb.position +  moveDirection * (moveSpeed * Time.fixedDeltaTime));
    }
}
