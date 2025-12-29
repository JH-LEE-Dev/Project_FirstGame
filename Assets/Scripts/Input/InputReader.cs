using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader
{
    public event Action<Vector2> MoveEvent;

    private InputActionSystem actions;

    public void Initialize()
    {
        if (actions == null)
        {
            actions = new InputActionSystem();

            actions.Combat.Move.performed += OnMove;
            actions.Combat.Move.canceled += OnMove;
        }

        actions.Combat.Enable();
    }

    public void Release()
    {
        actions.Combat.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();

        MoveEvent?.Invoke(move);
    }
}
