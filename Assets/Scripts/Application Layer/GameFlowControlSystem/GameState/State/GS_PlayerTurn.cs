using CardSystemSignals;
using GameControlSignals;
using System;
using UnitLogicSystemSignals;
using UnityEngine;

public class GS_PlayerTurn : GameState
{

    public override void Enter()
    {
        signalHub.Publish(new PlayerTurnStartSignal());
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
       
    }

    protected override void SubscribeEvents()
    {
        signalHub.Subscribe<PlayerTurnFinishedSignal>(PlayerTurnFinished);
    }

    protected override void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<PlayerTurnFinishedSignal>(PlayerTurnFinished);
    }

    private void PlayerTurnFinished(PlayerTurnFinishedSignal playerTurnFinishedSignal)
    {
        gameStateMachine.ChangeState<GS_EnemyTurn>();
    }
}
