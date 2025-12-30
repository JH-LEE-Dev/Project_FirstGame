using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateMachine
{
    private IState currentState;
    private Dictionary<Type, IState> states = new Dictionary<Type, IState>();

    public void AddState(IState state)
    {
        states[state.GetType()] = state;
    }

    public void ChangeState<T>() where T : IState
    {
        var type = typeof(T);

        if (!states.ContainsKey(type))
            throw new Exception($"{type} State is not registered.");

        currentState?.Exit();
        currentState = states[type];
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public bool IsState<T>() where T : IState
    {
        return currentState != null && currentState.GetType() == typeof(T);
    }
}
