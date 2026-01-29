using GameControlSignals;
using ShopSystemUISignals;
using UnityEngine;

public class GS_ShopTime : GameState
{
    public override void Enter()
    {
        signalHub.Publish(new ShopTimeStartedSignal());
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }

    protected override void SubscribeEvents()
    {
        signalHub.Subscribe<ShopIsClosedSignal>(ShopIsClosed);
    }

    protected override void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<ShopIsClosedSignal>(ShopIsClosed);
    }

    private void ShopIsClosed(ShopIsClosedSignal shopIsClosedSignal)
    {
        gameStateMachine.ChangeState<GS_WaveStarted>();
    }
}
