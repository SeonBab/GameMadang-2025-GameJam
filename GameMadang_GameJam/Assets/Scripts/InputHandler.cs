using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public InputSystem_Actions Input { get; private set; }

    public Vector2 MoveInput { get; private set; }

    void Awake()
    {
        Input = new InputSystem_Actions();

        Input.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        Input.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

    }

    void Start()
    {
        Input.Enable();
    }

    void OnDestroy()
    {
        Input.Disable();
    }
}
