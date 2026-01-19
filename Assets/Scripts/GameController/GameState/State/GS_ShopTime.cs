using ShopSystemSignal;
using UnityEngine;

public class GS_ShopTime : GameState
{
    public override void Enter()
    {
        signalHub.Publish(new ShopOpenedEvent());
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
