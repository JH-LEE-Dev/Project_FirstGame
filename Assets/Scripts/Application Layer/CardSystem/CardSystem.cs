using CardEffectSystemSignal;
using CardSystemSignals;
using GameControlSignals;
using System;
using UnitLogicSystemSignals;
using CardSystemUISignal;
using System.Diagnostics;
using UnityEngine;

public class CardSystem
{
    private SignalHub signalHub;
    private CardManager cardManager;
    private CardSystemController cardSystemController;
    private ComplexCardEffectResolver complexCardEffectResolver;
    private CardSelectionManager cardSelectionManager;

    public void Initialize(SignalHub _signalHub, CardManager _cardManager,
        CardSystemController _cardSystemController,CardSelectionManager _cardSelectionManager,
        ComplexCardEffectResolver _complexCardEffectResolver)
    {
        signalHub = _signalHub;
        cardManager = _cardManager;
        cardSystemController = _cardSystemController;
        cardSelectionManager = _cardSelectionManager;
        complexCardEffectResolver = _complexCardEffectResolver;

        SubscribeEvents();
        BindEvents();
    }

    private void SubscribeEvents()
    {
        signalHub.Subscribe<PlayerAttackFinishedSignal>(PlayerTurnFinished);
        signalHub.Subscribe<TryCardUseSignal>(TryCardUse);
        signalHub.Subscribe<PlayerTurnStartSignal>(StartCardDrawTurn);
        signalHub.Subscribe<CardUsingFinishedSignal>(CardUsingFinished);
        signalHub.Subscribe<DiscardBulletCardSignal>(DiscardBulletCard);
        signalHub.Subscribe<PlayerAttackedSignal>(PlayerAttacked);
        signalHub.Subscribe<WaveStartSignal>(WaveStarted);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<PlayerAttackFinishedSignal>(PlayerTurnFinished);
        signalHub.UnSubscribe<TryCardUseSignal>(TryCardUse);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(StartCardDrawTurn);
        signalHub.UnSubscribe<CardUsingFinishedSignal>(CardUsingFinished);
        signalHub.UnSubscribe<DiscardBulletCardSignal>(DiscardBulletCard);
        signalHub.UnSubscribe<PlayerAttackedSignal>(PlayerAttacked);
        signalHub.UnSubscribe<WaveStartSignal>(WaveStarted);
    }

    private void BindEvents()
    {
        cardManager.cardManagerEventInvoker.CardManagerEvent -= PublishCardSystemEvent;
        cardManager.cardManagerEventInvoker.CardManagerEvent += PublishCardSystemEvent;

        cardSystemController.CardDrawStartEvent -= CardDrawStarted;
        cardSystemController.CardDrawStartEvent += CardDrawStarted;

        cardSystemController.CardDrawFinishedEvent -= CardDrawFinished;
        cardSystemController.CardDrawFinishedEvent += CardDrawFinished;

        cardSystemController.SystemCommandDispatchEvent -= cardManager.ExecuteCommand;
        cardSystemController.SystemCommandDispatchEvent += cardManager.ExecuteCommand;

        cardSystemController.StatusCommandDispatchEvent -= CardStatusEffectDispatch;
        cardSystemController.StatusCommandDispatchEvent += CardStatusEffectDispatch;

        cardSystemController.SelectionSystemCommandDispatchEvent -= cardSelectionManager.ExecuteCommand;
        cardSystemController.SelectionSystemCommandDispatchEvent += cardSelectionManager.ExecuteCommand;

        cardSystemController.CardActionEndScopeEvent -= CardActionEndScope;
        cardSystemController.CardActionEndScopeEvent += CardActionEndScope;

        cardSystemController.ComplexCommandDispatchEvent -= complexCardEffectResolver.ExecuteCommand;
        cardSystemController.ComplexCommandDispatchEvent += complexCardEffectResolver.ExecuteCommand;

        cardSystemController.CardSlotCntChangedEvent -= CardSlotCntChanged;
        cardSystemController.CardSlotCntChangedEvent += CardSlotCntChanged;

        cardSelectionManager.CardSelectionStartEvent -= CardSelectionModeStart;
        cardSelectionManager.CardSelectionStartEvent += CardSelectionModeStart;
    }

    private void ReleaseEvents()
    {
        cardManager.cardManagerEventInvoker.CardManagerEvent -= PublishCardSystemEvent;

        cardSystemController.CardDrawStartEvent -= CardDrawStarted;

        cardSystemController.CardDrawFinishedEvent -= CardDrawFinished;

        cardSystemController.SystemCommandDispatchEvent -= cardManager.ExecuteCommand;

        cardSystemController.StatusCommandDispatchEvent -= CardStatusEffectDispatch;

        cardSystemController.SelectionSystemCommandDispatchEvent -= cardSelectionManager.ExecuteCommand;

        cardSystemController.CardActionEndScopeEvent -= CardActionEndScope;

        cardSystemController.ComplexCommandDispatchEvent -= complexCardEffectResolver.ExecuteCommand;

        cardSystemController.CardSlotCntChangedEvent -= CardSlotCntChanged;

        cardSelectionManager.CardSelectionStartEvent -= CardSelectionModeStart;
    }

    public void Release()
    {
        UnSubscribeEvents();
        ReleaseEvents();
    }

    private void PlayerTurnFinished(PlayerAttackFinishedSignal playerAttackFinishedSignal)
    {
        cardManager.PlayerTurnFinished();
    }

    private void CardUsingFinished(CardUsingFinishedSignal cardUsingFinishedSignal)
    {
        cardSystemController.CardUsingFinished();
    }

    private void PublishCardSystemEvent(CardSystemEventData data, ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new CardSystemEventSignal(data), cards);
    }

    private void CardDrawStarted()
    {
        signalHub.Publish(new CardDrawStartSignal());
    }

    private void CardActionEndScope()
    {
        signalHub.EndScope<CardActionScopeSignal>(new CardActionScopeSignal());
    }

    private void CardDrawFinished()
    {
        signalHub.Publish(new CardDrawFinishedSignal());
    }

    private void CardStatusEffectDispatch(CardSystemCommand command)
    {
        signalHub.Publish(new CardStatusEffectCommandDispatchSignal(command));
    }

    private void StartCardDrawTurn(PlayerTurnStartSignal playerTurnStartSignal)
    {
        cardSystemController.StartCardDrawTurn();
    }

    private void TryCardUse(TryCardUseSignal tryCardUseSignal)
    {
        CardUsedResult result = cardSystemController.TryCardUse(tryCardUseSignal.usedCard);

        signalHub.Publish(new CardUsedSignal(result.bVerified, result.slotIdx));
    }

    private void DiscardBulletCard(DiscardBulletCardSignal discardBulletCardSignal)
    {
        cardSystemController.DiscardBulletCard(discardBulletCardSignal.slotIdx);
    }

    private void PlayerAttacked(PlayerAttackedSignal playerAttackedSignal)
    {
        cardSystemController.ClearAllBulletCard();
    }

    private void WaveStarted(WaveStartSignal waveStartSignal)
    {
        cardSystemController.GameStarted();
    }

    private void CardSlotCntChanged(int cnt)
    {
        signalHub.Publish(new CardSlotCntChangedSignal(cnt));
    }

    private void CardSelectionModeStart(CardSelectionModeData data)
    {
        signalHub.Publish(new CardSelectionModeStartSignal(data));
    }
}
