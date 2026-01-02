using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader
{
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> PointerPositionEvent;

    private InputActionSystem actions;

    public void Initialize()
    {
        if (actions == null)
        {
            actions = new InputActionSystem();

            actions.Combat.Move.performed += OnMove;
            actions.Combat.Move.canceled += OnMove;
            actions.Combat.PointerPositioned.performed += OnPointerPosition;
        }

        actions.Combat.Enable();
    }

    public void Release()
    {
        actions.Combat.Disable();
        actions.Combat.Move.performed -= OnMove;
        actions.Combat.Move.canceled -= OnMove;
        actions.Combat.PointerPositioned.canceled -= OnPointerPosition;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();

        MoveEvent?.Invoke(move);
    }

    private void OnPointerPosition(InputAction.CallbackContext context)
    {
        Vector2 pos = context.ReadValue<Vector2>();
        PointerPositionEvent?.Invoke(pos);
    }
}
