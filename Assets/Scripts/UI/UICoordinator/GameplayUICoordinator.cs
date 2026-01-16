using CardSystemSignals;
using GameControlSignals;
using UnitLogicSystemSignals;
using UnitSpawnSystemSignals;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using WaveSystemSignals;

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
        signalHub.Subscribe<EnemyTurnStartEvent>(EnemyTurnStarted);
        signalHub.Subscribe<PlayerTurnStartEvent>(PlayerTurnStarted);
        signalHub.Subscribe<CardDrawFinishedEvent>(CardUseTimeStarted);
        signalHub.Subscribe<PlayerSpawnedEvent>(PlayerSpawned);
        signalHub.Subscribe<CardUsingTurnFinishedEvent>(CardUsingFinished);
        signalHub.Subscribe<PlayerTakeDamageEvent>(OnPlayerHit);
        signalHub.Subscribe<WaveStartEvent>(WaveStarted);
        signalHub.Subscribe<WaveEndEvent>(WaveEnded);
        signalHub.Subscribe<GameStartedEvent>(GameStarted);
        signalHub.Subscribe<EnemyIsDeadEvent>(EnemyIsDead);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<EnemyTurnStartEvent>(EnemyTurnStarted);
        signalHub.UnSubscribe<PlayerTurnStartEvent>(PlayerTurnStarted);
        signalHub.UnSubscribe<CardDrawFinishedEvent>(CardUseTimeStarted);
        signalHub.UnSubscribe<PlayerSpawnedEvent>(PlayerSpawned);
        signalHub.UnSubscribe<CardUsingTurnFinishedEvent>(CardUsingFinished);
        signalHub.UnSubscribe<PlayerTakeDamageEvent>(OnPlayerHit);
        signalHub.UnSubscribe<WaveStartEvent>(WaveStarted);
        signalHub.UnSubscribe<WaveEndEvent>(WaveEnded);
        signalHub.UnSubscribe<GameStartedEvent>(GameStarted);
        signalHub.UnSubscribe<EnemyIsDeadEvent>(EnemyIsDead);
    }

    public void Release()
    {
        UnSubscribeEvents();
    }

    public void PlayerTurnStarted(PlayerTurnStartEvent playerTurnStartEvent)
    {
        hudUISystem.PlayerTurnStarted();
    }

    public void EnemyTurnStarted(EnemyTurnStartEvent enemyTurnStartEvent)
    {
        hudUISystem.EnemyTurnStarted();
        gameplayUISystem.EnemyTurnStarted();
    }

    public void CardUseTimeStarted(CardDrawFinishedEvent cardDrawFinishedEvent)
    {
        hudUISystem.CardUseTimeStarted();
    }

    public void PlayerSpawned(PlayerSpawnedEvent playerSpawnedEvent)
    {
        hudUISystem.PlayerSpawned(playerSpawnedEvent.playerData);
    }

    public void CardUsingFinished(CardUsingTurnFinishedEvent cardUsingTurnFinishedEvent)
    {
        gameplayUISystem.CardUsingFinished();
    }

    public void OnPlayerHit(PlayerTakeDamageEvent playerTakeDamageEvent)
    {
        hudUISystem.OnPlayerHit(playerTakeDamageEvent.damage);
    }

    private void WaveStarted(WaveStartEvent waveStartEvent)
    {
        hudUISystem.WaveStarted(waveStartEvent.waveIdx);
    }

    private void GameStarted(GameStartedEvent gameStartedEvent)
    {
        hudUISystem.GameStarted();
    }

    private void WaveEnded(WaveEndEvent waveEndEvent)
    {
        hudUISystem.WaveEnded();
    }

    private void EnemyIsDead(EnemyIsDeadEvent enemyIsDeadEvent)
    {
        hudUISystem.EnemyIsDead(enemyIsDeadEvent.position);
    }
}
