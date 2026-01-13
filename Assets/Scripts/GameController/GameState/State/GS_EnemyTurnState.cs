using System;
using UnityEngine;

public class GS_EnemyTurnState : IState
{
    public event Action EnemyTurnStartEvent;

    private int waveIdx = 0;

    public void Enter()
    {
        EnemyTurnStartEvent?.Invoke();
    }

    public void Exit()
    {

    }

    public void Initialize()
    {

    }

    public void SetWaveIdx(int idx)
    {
        waveIdx = idx;
    }

    public void Update()
    {

    }

    public void Release()
    {
        EnemyTurnStartEvent = null;
    }
}
