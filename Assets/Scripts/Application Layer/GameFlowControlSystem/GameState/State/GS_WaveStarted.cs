using GameControlSignals;
using UnityEngine;
using WaveSystemSignals;

public class GS_WaveStarted : GameState
{
    private float proceedDelay = 1f;

    public override void Enter()
    {
        signalHub.Publish(new WaveStartEvent(waveIdx));

        ProceedToPlayerTurn();
    }

    public async void ProceedToPlayerTurn()
    {
        await Awaitable.WaitForSecondsAsync(proceedDelay);

        signalHub.Publish(new StartSpawnWaveEvent(waveIdx));
        ++waveIdx;

        gameStateMachine.ChangeState<GS_PlayerTurn>();
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
