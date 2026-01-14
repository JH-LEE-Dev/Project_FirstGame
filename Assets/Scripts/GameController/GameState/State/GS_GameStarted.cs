using System;
using System.Collections;

using UnityEngine;

//gameStateMachine 인터페이스 추상화할 것.
public class GS_GameStarted : GameState
{
    public event Action SpawnUnitsEvent;

    private float unitSpawnDelay = 1f;

    public async void SpawnUnitsTask()
    {
        await Awaitable.WaitForSecondsAsync(unitSpawnDelay);

        SpawnUnitsEvent?.Invoke();

        gameStateMachine.ChangeState<GS_PlayerTurnState>();
    }

    public override void SetWaveIdx(int idx)
    {

    }

    public override void Initialize(GameStateMachine stateMachine)
    {
        gameStateMachine = stateMachine;
    }

    public override void Enter()
    {
        SpawnUnitsTask();
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }

    public override void Release()
    {
        SpawnUnitsEvent = null;
    }
}
