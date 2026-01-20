using GameControlSignals;
using System;
using System.Collections;

using UnityEngine;

//gameStateMachine 인터페이스 추상화할 것.
public class GS_GameStarted : GameState
{
    private float unitSpawnDelay = 2f;

    public async void SpawnUnitsTask()
    {
        await Awaitable.WaitForSecondsAsync(unitSpawnDelay);

        gameStateMachine.ChangeState<GS_WaveStarted>();
    }

    public override void Enter()
    {
        signalHub.Publish(new GameStartedSignal());

        SpawnUnitsTask();
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }

    protected override void SubscribeEvents()
    {

    }

    protected override void UnSubscribeEvents()
    {

    }
}
