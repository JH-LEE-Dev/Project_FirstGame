using GameControlSignals;
using System;
using UnitLogicSystemSignals;
using UnityEngine;

public class GS_PlayerTurn : GameState
{

    public override void Enter()
    {
        signalHub.Publish(new PlayerTurnStartEvent());
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
       
    }

    protected override void SubscribeEvents()
    {
        signalHub.Subscribe<PlayerAttackFinishedEvent>(PlayerTurnFinished);
    }

    protected override void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<PlayerAttackFinishedEvent>(PlayerTurnFinished);
    }

    private void PlayerTurnFinished(PlayerAttackFinishedEvent playerTurnFinishedEvent)
    {
        gameStateMachine.ChangeState<GS_EnemyTurn>();
    }
}
