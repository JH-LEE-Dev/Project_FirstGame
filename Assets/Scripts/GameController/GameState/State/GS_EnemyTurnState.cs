using System;
using UnityEngine;

public class GS_EnemyTurnState : GameState
{
    public event Action EnemyTurnStartEvent;

    private int waveIdx = 0;

    public override void Enter()
    {
        EnemyTurnStartEvent?.Invoke();
    }

    public override void Exit()
    {

    }

    public override void Initialize(GameStateMachine stateMachine)
    {
        gameStateMachine = stateMachine;
    }

    public override void SetWaveIdx(int idx)
    {
        waveIdx = idx;
    }

    public override void Update()
    {

    }

    public override void Release()
    {
        EnemyTurnStartEvent = null;
    }
}
