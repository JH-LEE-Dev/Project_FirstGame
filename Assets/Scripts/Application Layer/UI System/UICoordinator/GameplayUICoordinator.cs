using CardSystemSignals;
using GameControlSignals;
using UnitLogicSystemSignals;
using UnitSpawnSystemSignals;
using WaveSystemSignals;
using CardSystemUISignal;

public class GameplayUICoordinator
{
    private SignalHub signalHub;
    private UIView_HUD hudUISystem;
    private UIView_Unit unitUISystem;
    private UIView_Gameplay gameplayUISystem;

    public void Initialize(SignalHub _signalHub,UIView_HUD _hudUISystem, UIView_Unit _unitUISystem,UIView_Gameplay _gameplayUISystem)
    {
        signalHub = _signalHub;
        hudUISystem = _hudUISystem;
        unitUISystem = _unitUISystem;
        gameplayUISystem = _gameplayUISystem;

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        signalHub.Subscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.Subscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.Subscribe<CardDrawFinishedSignal>(CardUseTimeStarted);
        signalHub.Subscribe<PlayerSpawnedSignal>(PlayerSpawned);
        signalHub.Subscribe<CardUsingFinishedSignal>(CardUsingFinished);
        signalHub.Subscribe<PlayerTakeDamageSignal>(OnPlayerHit);
        signalHub.Subscribe<PlayerGetShieldSignal>(PlayerGetShield);
        signalHub.Subscribe<WaveStartSignal>(WaveStarted);
        signalHub.Subscribe<WaveEndSignal>(WaveEnded);
        signalHub.Subscribe<GameStartedSignal>(GameStarted);
        signalHub.Subscribe<WaveProgressUpdatedSignal>(EnemyIsDead);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.UnSubscribe<CardDrawFinishedSignal>(CardUseTimeStarted);
        signalHub.UnSubscribe<PlayerSpawnedSignal>(PlayerSpawned);
        signalHub.UnSubscribe<CardUsingFinishedSignal>(CardUsingFinished);
        signalHub.UnSubscribe<PlayerTakeDamageSignal>(OnPlayerHit);
        signalHub.UnSubscribe<PlayerGetShieldSignal>(PlayerGetShield);
        signalHub.UnSubscribe<WaveStartSignal>(WaveStarted);
        signalHub.UnSubscribe<WaveEndSignal>(WaveEnded);
        signalHub.UnSubscribe<GameStartedSignal>(GameStarted);
        signalHub.UnSubscribe<WaveProgressUpdatedSignal>(EnemyIsDead);
    }

    public void Release()
    {
        UnSubscribeEvents();
    }

    public void PlayerTurnStarted(PlayerTurnStartSignal playerTurnStartSignal)
    {
        hudUISystem.PlayerTurnStarted();
    }

    public void EnemyTurnStarted(EnemyTurnStartSignal enemyTurnStartSignal)
    {
        hudUISystem.EnemyTurnStarted();
        gameplayUISystem.EnemyTurnStarted();
    }

    public void CardUseTimeStarted(CardDrawFinishedSignal cardDrawFinishedSignal)
    {
        hudUISystem.CardUseTimeStarted();
    }

    public void PlayerSpawned(PlayerSpawnedSignal playerSpawnedSignal)
    {
        hudUISystem.PlayerSpawned(playerSpawnedSignal.playerData);
    }

    public void CardUsingFinished(CardUsingFinishedSignal cardUsingFinishedSignal)
    {
        gameplayUISystem.CardUsingFinished();
    }

    public void OnPlayerHit(PlayerTakeDamageSignal playerTakeDamageSignal)
    {
        hudUISystem.OnPlayerHit(playerTakeDamageSignal.damage);
    }

    private void WaveStarted(WaveStartSignal waveStartSignal)
    {
        hudUISystem.WaveStarted(waveStartSignal.waveIdx);
    }

    private void GameStarted(GameStartedSignal gameStartedSignal)
    {
        hudUISystem.GameStarted();
    }

    private void WaveEnded(WaveEndSignal waveEndSignal)
    {
        hudUISystem.WaveEnded();
    }

    private void EnemyIsDead(WaveProgressUpdatedSignal waveProgressUpdatedSignal)
    {
        hudUISystem.EnemyIsDead(waveProgressUpdatedSignal.position);
    }

    private void PlayerGetShield(PlayerGetShieldSignal playerGetShieldSignal)
    {
        hudUISystem.PlayerGetShield(playerGetShieldSignal.amount);
    }
}
