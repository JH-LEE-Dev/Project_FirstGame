using UnityEngine;

public abstract class GameState
{
    protected GameStateMachine gameStateMachine;
    public abstract void SetWaveIdx(int idx);
    public abstract void Initialize(GameStateMachine stateMachine);
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();

    public abstract void Release();
}