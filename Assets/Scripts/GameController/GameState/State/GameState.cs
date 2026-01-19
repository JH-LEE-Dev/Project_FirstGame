using UnityEngine;

public abstract class GameState
{
    protected GameStateMachine gameStateMachine;
    protected SignalHub signalHub;
    protected int waveIdx;
    public void SetWaveIdx(int idx) { waveIdx = idx; }
    public void Initialize(SignalHub _signalHub, GameStateMachine _gameStateMachine)
    {
        signalHub = _signalHub; gameStateMachine = _gameStateMachine;
        SubscribeEvents();
    }
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();

    public void Release() { UnSubscribeEvents(); }

    protected abstract void SubscribeEvents();
    protected abstract void UnSubscribeEvents();
}