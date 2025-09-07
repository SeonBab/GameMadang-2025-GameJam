using System;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector2 MoveInput; //  { get; set; }
    public InputSystem_Actions Input { get; private set; }

    static public Action OnRemoveInputCallbacks;

    private void Awake()
    {
        Input = new InputSystem_Actions();
    }

    private void Start()
    {
        Input.Player.Move.performed += Move;
        Input.Player.Move.canceled += Move;

        Input.UI.Escape.performed += UIManager.Instance.TogglePauseMenu;

        OnRemoveInputCallbacks += RemoveInputCallbacks;
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
        RemoveInputCallbacks();
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        var x = Mathf.Round(ctx.ReadValue<Vector2>().x);
        var y = Mathf.Round(ctx.ReadValue<Vector2>().y);

        MoveInput.Set(x, y);
    }

    public void RemoveInputCallbacks()
    {
        Input.Player.Move.performed -= Move;
        Input.Player.Move.canceled -= Move;

        MoveInput = new Vector2(0f, 0f);

        Input.UI.Escape.performed -= UIManager.Instance.TogglePauseMenu;
    }
}