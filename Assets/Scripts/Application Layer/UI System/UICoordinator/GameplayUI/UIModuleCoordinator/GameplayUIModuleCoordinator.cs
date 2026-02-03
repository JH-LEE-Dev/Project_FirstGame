using CardSystemSignals;
using CardSystemUISignal;
using GameControlSignals;
using UICommandSystemSignals;
using UnitLogicSystemSignals;
using UnitSpawnSystemSignals;
using WaveSystemSignals;
using System.Collections.Generic;

public class GameplayUIModuleCoordinator
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
        signalHub.Subscribe<CardUsePhaseStarted>(CardUsePhaseStarted);
        signalHub.Subscribe<CardSystem_ActionDispatchSignal>(RecieveUIJob);
        signalHub.Subscribe<CardUsedSignal>(CardUsed);
        signalHub.Subscribe<CardSelectionModeStartSignal>(CardSelectionModeStarted);
        signalHub.Subscribe<ShopTimeStartedSignal>(ShopTimeStarted);

        //For GameplayUICoordinator
        signalHub.Subscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.Subscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.Subscribe<CardUsePhaseStarted>(CardUsePhaseStarted);
        signalHub.Subscribe<CardUsingFinishedSignal>(CardUsingFinished);
        signalHub.Subscribe<PlayerTakeDamageSignal>(OnPlayerHit);
        signalHub.Subscribe<PlayerGetShieldSignal>(PlayerGetShield);
        signalHub.Subscribe<PlayerGetHPSignal>(PlayerGetHP);
        signalHub.Subscribe<WaveStartSignal>(WaveStarted);
        signalHub.Subscribe<WaveEndSignal>(WaveEnded);
        signalHub.Subscribe<GameStartedSignal>(GameStarted);
        signalHub.Subscribe<CardSlotCntChangedSignal>(CardSlotCntChanged);
        signalHub.Subscribe<PlayerAttackedSignal>(PlayerAttacked);
        signalHub.Subscribe<CardSelectionModeStartSignal>(CardSelectionModeStarted);
        signalHub.Subscribe<EnemyTakeDamageSignal>(EnemyTakeDamage);
        signalHub.Subscribe<WaveProgressUpdatedSignal>(EnemyIsKilled);
        signalHub.Subscribe<ResetPlayerShieldSignal>(ResetPlayerShield);
        signalHub.Subscribe<CharacterStatChangedSignal>(CharacterStatChanged);
    }

    private void UnSubscribeEvents()
    {
        //For CardUICoordinator
        signalHub.UnSubscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.UnSubscribe<CardUsePhaseStarted>(CardUsePhaseStarted);
        signalHub.UnSubscribe<CardSystem_ActionDispatchSignal>(RecieveUIJob);
        signalHub.UnSubscribe<CardUsedSignal>(CardUsed);
        signalHub.UnSubscribe<CardSelectionModeStartSignal>(CardSelectionModeStarted);
        signalHub.UnSubscribe<ShopTimeStartedSignal>(ShopTimeStarted);

        //For GameplayUICoordinator
        signalHub.UnSubscribe<EnemyTurnStartSignal>(EnemyTurnStarted);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(PlayerTurnStarted);
        signalHub.UnSubscribe<CardUsePhaseStarted>(CardUsePhaseStarted);
        signalHub.UnSubscribe<CardUsingFinishedSignal>(CardUsingFinished);
        signalHub.UnSubscribe<PlayerTakeDamageSignal>(OnPlayerHit);
        signalHub.UnSubscribe<PlayerGetShieldSignal>(PlayerGetShield);
        signalHub.UnSubscribe<PlayerGetHPSignal>(PlayerGetHP);
        signalHub.UnSubscribe<WaveStartSignal>(WaveStarted);
        signalHub.UnSubscribe<WaveEndSignal>(WaveEnded);
        signalHub.UnSubscribe<GameStartedSignal>(GameStarted);
        signalHub.UnSubscribe<CardSlotCntChangedSignal>(CardSlotCntChanged);
        signalHub.UnSubscribe<PlayerAttackedSignal>(PlayerAttacked);
        signalHub.UnSubscribe<EnemyTakeDamageSignal>(EnemyTakeDamage);
        signalHub.UnSubscribe<WaveProgressUpdatedSignal>(EnemyIsKilled);
        signalHub.UnSubscribe<ResetPlayerShieldSignal>(ResetPlayerShield);
        signalHub.UnSubscribe<CharacterStatChangedSignal>(CharacterStatChanged);
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

        cardUICoordinator.CardSelectionEndEvent -= CardSelectionEnd;
        cardUICoordinator.CardSelectionEndEvent += CardSelectionEnd;
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

        cardUICoordinator.CardSelectionEndEvent -= CardSelectionEnd;
    }

    private void UnEquipBulletCard(int slotIdx)
    {
        signalHub.Publish(new DiscardBulletCardSignal(slotIdx));

        cardUICoordinator.UnEquipBulletCard(slotIdx);
    }

    public void TryCardUse(ICardDataInstanceProvider usedCard)
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

    public void RecieveUIJob(CardSystem_ActionDispatchSignal cardSystem_JobDispatchSignal)
    {
        cardUICoordinator.RecieveUIJob(cardSystem_JobDispatchSignal.actionDataBatch);
    }

    public void CardUsePhaseStarted(CardUsePhaseStarted cardDrawFinishedSignal)
    {
        cardUICoordinator.CardUsePhaseStarted();
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

    private void WaveEnded(WaveEndSignal waveEndSignal)
    {
        gameplayUICoordinator.WaveEnded();
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

    public void CardSelectionModeStarted(CardSelectionModeStartSignal cardSelectionModeStartSignal)
    {
        cardUICoordinator.CardSelectionModeStarted(cardSelectionModeStartSignal.data);
    }

    private void CardSelectionEnd(CardSelectionModeData _data,List<ICardDataInstanceProvider> _cards)
    {
        signalHub.Publish(new UICardSelectionEndSignal(_data, _cards));
    }

    private void EnemyTakeDamage(EnemyTakeDamageSignal enemyTakeDamageSignal)
    {
        gameplayUICoordinator.EnemyTakeDamage(enemyTakeDamageSignal.enemyData, enemyTakeDamageSignal.damage,
            enemyTakeDamageSignal.bCritical);
    }

    private void EnemyIsKilled(WaveProgressUpdatedSignal waveProgressUpdatedSignal)
    {
        gameplayUICoordinator.EnemyIsKilled(waveProgressUpdatedSignal.enemyData);
    }

    private void ResetPlayerShield(ResetPlayerShieldSignal resetPlayerShield)
    {
        gameplayUICoordinator.ResetPlayerShield();
    }

    private void CharacterStatChanged(CharacterStatChangedSignal characterStatChangedSignal)
    {
        gameplayUICoordinator.CharacterStatChanged();
    }

    private void ShopTimeStarted(ShopTimeStartedSignal shopTimeStartedSignal)
    {
        cardUICoordinator.ShopTimeStarted();
    }
}
