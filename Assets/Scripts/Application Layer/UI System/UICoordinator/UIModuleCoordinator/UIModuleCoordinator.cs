using CardSystemSignals;
using CardSystemUISignal;
using GameControlSignals;
using UICommandSystemSignals;
using UnitLogicSystemSignals;
using UnitSpawnSystemSignals;
using UnityEngine;
using WaveSystemSignals;

public class UIModuleCoordinator
{
    private CardUICoordinator cardUICoordinator;
    private GameplayUICoordinator gameplayUICoordinator;
    private SignalHub signalHub;

    public void Initialize(SignalHub _signalHub, CardUICoordinator _cardUICoordinator, GameplayUICoordinator _gameplayUICoordinator)
    {
        signalHub = _signalHub;
        cardUICoordinator = _cardUICoordinator;
        gameplayUICoordinator = _gameplayUICoordinator;

        BindEvents();
        SubscribeEvents();
    }

    public void Release()
    {
        ReleaseEvents();
        UnSubscribeEvents();
    }

    private void SubscribeEvents()
    {
        //For CardUICoordinator
        signalHub.Subscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.Subscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.Subscribe<CardDrawFinishedSignal>(CardDrawFinished);
        signalHub.Subscribe<CardSystem_JobDispatchSignal>(RecieveUIJob);

        //For GameplayUICoordinator
        signalHub.Subscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.Subscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.Subscribe<CardDrawFinishedSignal>(CardUseTimeStarted);
        signalHub.Subscribe<PlayerSpawnedSignal>(PlayerSpawned);
        signalHub.Subscribe<CardUsingFinishedSignal>(CardUsingFinished);
        signalHub.Subscribe<PlayerTakeDamageSignal>(OnPlayerHit);
        signalHub.Subscribe<PlayerGetShieldSignal>(PlayerGetShield);
        signalHub.Subscribe<PlayerGetHPSignal>(PlayerGetHP);
        signalHub.Subscribe<WaveStartSignal>(WaveStarted);
        signalHub.Subscribe<WaveEndSignal>(WaveEnded);
        signalHub.Subscribe<GameStartedSignal>(GameStarted);
        signalHub.Subscribe<WaveProgressUpdatedSignal>(EnemyIsDead);
        signalHub.Subscribe<CardSlotCntChangedSignal>(CardSlotCntChanged);
        signalHub.Subscribe<CharacterSpawnedSignal>(CharacterSpawned);
        signalHub.Subscribe<PlayerAttackedSignal>(PlayerAttacked);
        signalHub.Subscribe<CardUsedSignal>(CardUsed);
    }

    private void UnSubscribeEvents()
    {
        //For CardUICoordinator
        signalHub.UnSubscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.UnSubscribe<CardDrawFinishedSignal>(CardDrawFinished);
        signalHub.UnSubscribe<CardSystem_JobDispatchSignal>(RecieveUIJob);

        //For GameplayUICoordinator
        signalHub.UnSubscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.UnSubscribe<CardDrawFinishedSignal>(CardUseTimeStarted);
        signalHub.UnSubscribe<PlayerSpawnedSignal>(PlayerSpawned);
        signalHub.UnSubscribe<CardUsingFinishedSignal>(CardUsingFinished);
        signalHub.UnSubscribe<PlayerTakeDamageSignal>(OnPlayerHit);
        signalHub.UnSubscribe<PlayerGetShieldSignal>(PlayerGetShield);
        signalHub.UnSubscribe<PlayerGetHPSignal>(PlayerGetHP);
        signalHub.UnSubscribe<WaveStartSignal>(WaveStarted);
        signalHub.UnSubscribe<WaveEndSignal>(WaveEnded);
        signalHub.UnSubscribe<GameStartedSignal>(GameStarted);
        signalHub.UnSubscribe<WaveProgressUpdatedSignal>(EnemyIsDead);
        signalHub.UnSubscribe<CardSlotCntChangedSignal>(CardSlotCntChanged);
        signalHub.UnSubscribe<CharacterSpawnedSignal>(CharacterSpawned);
        signalHub.UnSubscribe<PlayerAttackedSignal>(PlayerAttacked);
        signalHub.UnSubscribe<CardUsedSignal>(CardUsed);
    }

    public void BindEvents()
    {
        cardUICoordinator.CardEquippedEvent -= gameplayUICoordinator.EquipBulletCard;
        cardUICoordinator.CardEquippedEvent += gameplayUICoordinator.EquipBulletCard;

        gameplayUICoordinator.UnEquipBulletCardEvent -= UnEquipBulletCard;
        gameplayUICoordinator.UnEquipBulletCardEvent += UnEquipBulletCard;

        gameplayUICoordinator.CancelCardPreviewEvent -= cardUICoordinator.CancelPreview;
        gameplayUICoordinator.CancelCardPreviewEvent += cardUICoordinator.CancelPreview;

        cardUICoordinator.TryCardUseEvent -= TryCardUse;
        cardUICoordinator.TryCardUseEvent += TryCardUse;

        cardUICoordinator.CardUsingFinishedEvent -= CardUsingFinished;
        cardUICoordinator.CardUsingFinishedEvent += CardUsingFinished;

        cardUICoordinator.UICommandCompleteEvent -= UICommandComplete;
        cardUICoordinator.UICommandCompleteEvent += UICommandComplete;

        gameplayUICoordinator.CardUsedEvent -= cardUICoordinator.CardUsed;
        gameplayUICoordinator.CardUsedEvent += cardUICoordinator.CardUsed;
    }

    public void ReleaseEvents()
    {
        cardUICoordinator.CardEquippedEvent -= gameplayUICoordinator.EquipBulletCard;

        gameplayUICoordinator.UnEquipBulletCardEvent -= cardUICoordinator.UnEquipBulletCard;

        gameplayUICoordinator.CancelCardPreviewEvent -= cardUICoordinator.CancelPreview;

        cardUICoordinator.TryCardUseEvent -= TryCardUse;

        cardUICoordinator.CardUsingFinishedEvent -= CardUsingFinished;

        cardUICoordinator.UICommandCompleteEvent -= UICommandComplete;

        gameplayUICoordinator.CardUsedEvent -= cardUICoordinator.CardUsed;
    }

    private void UnEquipBulletCard(int slotIdx)
    {
        signalHub.Publish(new DiscardBulletCardSignal(slotIdx));

        cardUICoordinator.UnEquipBulletCard(slotIdx);
    }

    public void TryCardUse(CardDataInstance usedCard)
    {
        signalHub.Publish(new TryCardUseSignal(usedCard));
    }

    public void CardUsingFinished()
    {
        signalHub.Publish(new CardUsingFinishedSignal());
    }

    public void EnemyTurnStarted(EnemyTurnStartSignal enemyTurnStartSignal)
    {
        cardUICoordinator.EnemyTurnStarted();
        gameplayUICoordinator.EnemyTurnStarted();
    }

    public void PlayerTurnStarted(PlayerTurnStartSignal playerTurnStartSignal)
    {
        cardUICoordinator.PlayerTurnStarted();
        gameplayUICoordinator.PlayerTurnStarted();
    }

    public void RecieveUIJob(CardSystem_JobDispatchSignal cardSystem_JobDispatchSignal)
    {
        cardUICoordinator.RecieveUIJob(cardSystem_JobDispatchSignal.actionDataBatch);
    }

    public void CardDrawFinished(CardDrawFinishedSignal cardDrawFinishedSignal)
    {
        cardUICoordinator.CardDrawFinished();
    }

    private void UICommandComplete(int idx)
    {
        signalHub.Publish(new UICommandCompleteSignal(idx));
    }

    private void WaveStarted(WaveStartSignal waveStartSignal)
    {
        gameplayUICoordinator.WaveStarted(waveStartSignal.waveIdx);
    }

    private void GameStarted(GameStartedSignal gameStartedSignal)
    {
        gameplayUICoordinator.GameStarted();
    }
    private void CharacterSpawned(CharacterSpawnedSignal characterSpawnedSignal)
    {
        gameplayUICoordinator.CharacterSpawned(characterSpawnedSignal.characterData);
    }

    private void WaveEnded(WaveEndSignal waveEndSignal)
    {
        gameplayUICoordinator.WaveEnded();
    }

    private void EnemyIsDead(WaveProgressUpdatedSignal waveProgressUpdatedSignal)
    {
        gameplayUICoordinator.EnemyIsDead(waveProgressUpdatedSignal.position);
    }

    private void PlayerGetShield(PlayerGetShieldSignal playerGetShieldSignal)
    {
        gameplayUICoordinator.PlayerGetShield(playerGetShieldSignal.amount);
    }

    private void CardSlotCntChanged(CardSlotCntChangedSignal cardSlotCntChangedSignal)
    {
        gameplayUICoordinator.CardSlotCntChanged(cardSlotCntChangedSignal.cnt);
    }

    private void CardUsed(CardUsedSignal cardUsedSignal)
    {
        gameplayUICoordinator.CardUsed(cardUsedSignal.bVerified, cardUsedSignal.slotIdx);
    }

    public void PlayerAttacked(PlayerAttackedSignal playerAttackedSignal)
    {
        gameplayUICoordinator.PlayerAttacked();
    }

    public void CardUseTimeStarted(CardDrawFinishedSignal cardDrawFinishedSignal)
    {
        gameplayUICoordinator.CardUseTimeStarted();
    }

    public void PlayerSpawned(PlayerSpawnedSignal playerSpawnedSignal)
    {
        gameplayUICoordinator.PlayerSpawned(playerSpawnedSignal.playerData);
    }

    public void CardUsingFinished(CardUsingFinishedSignal cardUsingFinishedSignal)
    {
        gameplayUICoordinator.CardUsingFinished();
    }

    public void OnPlayerHit(PlayerTakeDamageSignal playerTakeDamageSignal)
    {
        gameplayUICoordinator.OnPlayerHit(playerTakeDamageSignal.damage);
    }

    public void PlayerGetHP(PlayerGetHPSignal playerGetHPSignal)
    {
        gameplayUICoordinator.PlayerGetHP(playerGetHPSignal.amount);
    }
}
