using GameControlSignals;
using System;
using UnityEngine;
using WaveSystemSignals;

public class GS_EnemyTurn : GameState
{
    public override void Enter()
    {
        signalHub.Publish(new EnemyTurnStartSignal());
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }

    private void AllEnemyDead(AllEnemyDeadSignal allEnemyDeadSignal)
    {
        gameStateMachine.ChangeState<GS_WaveEnded>();
    }

    protected override void SubscribeEvents()
    {
        signalHub.Subscribe<AllEnemyDeadSignal>(AllEnemyDead);
    }

    protected override void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<AllEnemyDeadSignal>(AllEnemyDead);
    }
}
