using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateMachine
{
    private GameState currentState;
    private Dictionary<Type, GameState> states = new Dictionary<Type, GameState>();

    public void AddState(GameState state)
    {
        states[state.GetType()] = state;
    }

    public void ChangeState<T>() where T : GameState
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

    public bool IsState<T>() where T : GameState
    {
        return currentState != null && currentState.GetType() == typeof(T);
    }

    public T GetState<T>() where T : GameState
    {
        var type = typeof(T);

        if (!states.TryGetValue(type, out GameState instance) || instance == null)
        {
            return default(T);
        }

        return (T)instance;
    }
}
