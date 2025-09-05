using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector2 MoveInput; // { get; private set; }
    public InputSystem_Actions Input { get; private set; }

    private void Awake()
    {
        Input = new InputSystem_Actions();

        Input.Player.Move.performed += Move;
        Input.Player.Move.canceled += Move;
    }

    private void OnEnable()
    {
        Input.Enable();
    }

    private void OnDisable()
    {
        Input.Disable();
    }

    private void OnDestroy()
    {
        Input.Player.Move.performed -= Move;
        Input.Player.Move.canceled -= Move;
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        var x = Mathf.Round(ctx.ReadValue<Vector2>().x);
        var y = Mathf.Round(ctx.ReadValue<Vector2>().y);

        MoveInput.Set(x, y);
    }
}