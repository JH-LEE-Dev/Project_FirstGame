using System;
using UnityEngine;

public class GS_PlayerTurnState : IState
{
    public event Action<int> PlayerTurnStartEvent;

    private int waveIdx = 0;

    public void Enter()
    {
        PlayerTurnStartEvent?.Invoke(waveIdx);
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
        PlayerTurnStartEvent = null;
    }
}
