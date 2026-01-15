using GameControlSignals;
using System;
using UnityEngine;
using WaveSystemSignals;

public class GS_EnemyTurn : GameState
{
    public override void Enter()
    {
        signalHub.Publish(new EnemyTurnStartEvent());
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }

    private void AllEnemyDead(AllEnemyDeadEvent allEnemyDeadEvent)
    {
        gameStateMachine.ChangeState<GS_WaveEnded>();
    }

    protected override void SubscribeEvents()
    {
        signalHub.Subscribe<AllEnemyDeadEvent>(AllEnemyDead);
    }

    protected override void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<AllEnemyDeadEvent>(AllEnemyDead);
    }
}
