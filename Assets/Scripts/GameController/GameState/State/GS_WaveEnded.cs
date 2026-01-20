using UnityEngine;
using WaveSystemSignals;

public class GS_WaveEnded : GameState
{
    private float nextWaveDelay =1f;

    public override void Enter()
    {
        signalHub.Publish(new WaveEndEvent());
        MoveToNextWave();
    }

    public override void Exit()
    {

    }

    public async void MoveToNextWave()
    {
        await Awaitable.WaitForSecondsAsync(nextWaveDelay);

        gameStateMachine.ChangeState<GS_ShopTime>();
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
