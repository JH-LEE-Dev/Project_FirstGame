using System;
using UnityEngine;

public class GS_PlayerTurnState : GameState
{
    public event Action<int> PlayerTurnStartEvent;

    private int waveIdx = 0;

    public override void SetWaveIdx(int idx)
    {
        waveIdx = idx;
    }

    public override void Initialize(GameStateMachine stateMachine)
    {
        gameStateMachine = stateMachine;
    }

    public override void Enter()
    {
        PlayerTurnStartEvent?.Invoke(waveIdx);
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
       
    }

    public override void Release()
    {
        PlayerTurnStartEvent = null;
    }
}
