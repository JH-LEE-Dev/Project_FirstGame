using EffectSystemSignal;
using CardSystemSignals;
using GameControlSignals;
using System;
using UnitLogicSystemSignals;
using CardSystemUISignal;
using WaveSystemSignals;
using ShopSystemUISignals;

public class CardSystem
{
    private SignalHub signalHub;
    private CardManager cardManager;
    private CardSystemController cardSystemController;
    private ComplexCardEffectResolver complexCardEffectResolver;
    private CardSelectionManager cardSelectionManager;
    private CardDataControlManager cardDataControlManager;
    private ShopBehaviorHandler shopBehaviorHandler;
    private CardFlowDataManager cardFlowDataManager;

    public void Initialize(SignalHub _signalHub, CardManager _cardManager,
        CardSystemController _cardSystemController,CardSelectionManager _cardSelectionManager,
        ComplexCardEffectResolver _complexCardEffectResolver, CardDataControlManager _cardDataControlManager,
        ShopBehaviorHandler _shopBehaviorHandler,CardFlowDataManager _cardFlowDataManager)
    {
        signalHub = _signalHub;
        cardManager = _cardManager;
        cardSystemController = _cardSystemController;
        cardSelectionManager = _cardSelectionManager;
        complexCardEffectResolver = _complexCardEffectResolver;
        cardDataControlManager =_cardDataControlManager;
        shopBehaviorHandler = _shopBehaviorHandler;
        cardFlowDataManager = _cardFlowDataManager;

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
        signalHub.Subscribe<WaveEndSignal>(WaveEnd);
        signalHub.Subscribe<UICardSelectionEndSignal>(CardSelectionEnd);
        signalHub.Subscribe<ShopOutputSignal>(HandleShopOutput);
        signalHub.Subscribe<ArtifactEffectCommandDispatchSignal>(ArtifactCommandReceived);
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
        signalHub.UnSubscribe<WaveEndSignal>(WaveEnd);
        signalHub.UnSubscribe<UICardSelectionEndSignal>(CardSelectionEnd);
        signalHub.UnSubscribe<ShopOutputSignal>(HandleShopOutput);
        signalHub.UnSubscribe<ArtifactEffectCommandDispatchSignal>(ArtifactCommandReceived);
    }

    private void BindEvents()
    {
        cardManager.cardSystemEventInvoker.CardManagerEvent -= PublishCardLogicSystemEvent;
        cardManager.cardSystemEventInvoker.CardManagerEvent += PublishCardLogicSystemEvent;

        cardSystemController.CardDrawStartEvent -= CardDrawStarted;
        cardSystemController.CardDrawStartEvent += CardDrawStarted;

        cardSystemController.StartCardUsePhaseEvent -= CardUsePhaseStarted;
        cardSystemController.StartCardUsePhaseEvent += CardUsePhaseStarted;

        cardSystemController.StartAfterCardUsePhaseEvent -= AfterCardUsePhaseStarted;
        cardSystemController.StartAfterCardUsePhaseEvent += AfterCardUsePhaseStarted;

        cardSystemController.CardLogicSystemCommandDispatchEvent -= cardManager.ExecuteCommand;
        cardSystemController.CardLogicSystemCommandDispatchEvent += cardManager.ExecuteCommand;

        cardSystemController.CardDataControlSystemCommandDispatchEvent -= cardDataControlManager.ExecuteCommand;
        cardSystemController.CardDataControlSystemCommandDispatchEvent += cardDataControlManager.ExecuteCommand;

        cardSystemController.CardStatusCommandDispatchEvent -= CardStatusEffectDispatch;
        cardSystemController.CardStatusCommandDispatchEvent += CardStatusEffectDispatch;

        cardDataControlManager.cardSystemEventInvoker.CardDataControlManagerEvent -= PublishCardDataControlSystemEvent;
        cardDataControlManager.cardSystemEventInvoker.CardDataControlManagerEvent += PublishCardDataControlSystemEvent;

        cardSystemController.CardSelectionSystemCommandDispatchEvent -= cardSelectionManager.ExecuteCommand;
        cardSystemController.CardSelectionSystemCommandDispatchEvent += cardSelectionManager.ExecuteCommand;

        cardSystemController.CardActionEndScopeEvent -= CardActionEndScope;
        cardSystemController.CardActionEndScopeEvent += CardActionEndScope;

        cardSystemController.CardComplexCommandDispatchEvent -= complexCardEffectResolver.ExecuteCommand;
        cardSystemController.CardComplexCommandDispatchEvent += complexCardEffectResolver.ExecuteCommand;

        cardSystemController.CardSlotCntChangedEvent -= CardSlotCntChanged;
        cardSystemController.CardSlotCntChangedEvent += CardSlotCntChanged;

        cardSelectionManager.CardSelectionStartEvent -= CardSelectionModeStart;
        cardSelectionManager.CardSelectionStartEvent += CardSelectionModeStart;

        cardSelectionManager.RequestCardLogicSystemActionEvent -= cardSystemController.RequestCardLogicSystemActionCommand;
        cardSelectionManager.RequestCardLogicSystemActionEvent += cardSystemController.RequestCardLogicSystemActionCommand;

        cardSelectionManager.RequestCardDataControlSystemActionEvent -= cardSystemController.RequestCardDataControlSystemActionCommand;
        cardSelectionManager.RequestCardDataControlSystemActionEvent += cardSystemController.RequestCardDataControlSystemActionCommand;

        cardSystemController.PlayerTurnFinishedEvent -= PlayerTurnFinished;
        cardSystemController.PlayerTurnFinishedEvent += PlayerTurnFinished;

        shopBehaviorHandler.RequestCardDataControlSystemActionEvent -= cardSystemController.RequestCardDataControlSystemActionCommand;
        shopBehaviorHandler.RequestCardDataControlSystemActionEvent += cardSystemController.RequestCardDataControlSystemActionCommand;
    }

    private void ReleaseEvents()
    {
        cardManager.cardSystemEventInvoker.CardManagerEvent -= PublishCardLogicSystemEvent;

        cardDataControlManager.cardSystemEventInvoker.CardDataControlManagerEvent -= PublishCardDataControlSystemEvent;

        cardSystemController.CardDrawStartEvent -= CardDrawStarted;

        cardSystemController.StartCardUsePhaseEvent -= CardUsePhaseStarted;

        cardSystemController.StartAfterCardUsePhaseEvent -= AfterCardUsePhaseStarted;

        cardSystemController.CardLogicSystemCommandDispatchEvent -= cardManager.ExecuteCommand;

        cardSystemController.CardDataControlSystemCommandDispatchEvent -= cardDataControlManager.ExecuteCommand;

        cardSystemController.CardStatusCommandDispatchEvent -= CardStatusEffectDispatch;

        cardSystemController.CardSelectionSystemCommandDispatchEvent -= cardSelectionManager.ExecuteCommand;

        cardSystemController.CardActionEndScopeEvent -= CardActionEndScope;

        cardSystemController.CardComplexCommandDispatchEvent -= complexCardEffectResolver.ExecuteCommand;

        cardSystemController.CardSlotCntChangedEvent -= CardSlotCntChanged;

        cardSelectionManager.CardSelectionStartEvent -= CardSelectionModeStart;

        cardSelectionManager.RequestCardLogicSystemActionEvent -= cardSystemController.RequestCardLogicSystemActionCommand;

        cardSelectionManager.RequestCardDataControlSystemActionEvent -= cardSystemController.RequestCardDataControlSystemActionCommand;

        cardSystemController.PlayerTurnFinishedEvent -= PlayerTurnFinished;

        shopBehaviorHandler.RequestCardDataControlSystemActionEvent -= cardSystemController.RequestCardDataControlSystemActionCommand;
    }

    public void Release()
    {
        UnSubscribeEvents();
        ReleaseEvents();
    }

    private void PlayerTurnFinished(PlayerAttackFinishedSignal playerAttackFinishedSignal)
    {
        cardSystemController.PlayerTurnFinished();
    }

    private void CardUsingFinished(CardUsingFinishedSignal cardUsingFinishedSignal)
    {
        cardSystemController.CardUsingFinished();
    }

    private void PublishCardLogicSystemEvent(CardLogicSystemEventData data, ReadOnlySpan<CardDataInstance> cards = default)
    {
        cardFlowDataManager.CatchCardFlow(data, cards);
        cardSystemController.CatchCardLogicSystemEvent(data, cards);
        signalHub.Publish(new CardLogicSystemEventSignal(data), cards);
    }

    private void PublishCardDataControlSystemEvent(CardDataControlSystemEventData data, ReadOnlySpan<CardDataInstance> cards = default)
    {
        cardSystemController.CatchCardDataControlSystemEvent(data, cards);
        signalHub.Publish(new CardDataControlSystemEventSignal(data), cards);
    }

    private void CardDrawStarted()
    {
        signalHub.Publish(new CardDrawStartSignal());
    }

    private void CardActionEndScope()
    {
        signalHub.EndScope<CardActionScopeSignal>(new CardActionScopeSignal());
    }

    private void CardUsePhaseStarted()
    {
        signalHub.Publish(new CardUsePhaseStartedSignal());
    }

    private void AfterCardUsePhaseStarted()
    {
        signalHub.Publish(new AfterCardUsePhaseStartedSignal());
    }

    private void PlayerTurnFinished()
    {
        signalHub.Publish(new PlayerTurnFinishedSignal());
    }

    private void CardStatusEffectDispatch(GameSystemCommand command, bool bUndo)
    {
        signalHub.Publish(new CardStatusEffectCommandDispatchSignal(command,bUndo));
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

    }

    private void CardSelectionEnd(UICardSelectionEndSignal uICardSelectionEndSignal)
    {
        cardSelectionManager.CardSelectionEnd(uICardSelectionEndSignal.data, uICardSelectionEndSignal.cards);
    }

    private void CardSlotCntChanged(int cnt)
    {
        signalHub.Publish(new CardSlotCntChangedSignal(cnt));
    }

    private void CardSelectionModeStart(CardSelectionModeData data)
    {
        signalHub.Publish(new CardSelectionModeStartSignal(data));
    }

    private void WaveEnd(WaveEndSignal waveEndSignal)
    {
        cardSystemController.WaveEnded();
        cardSystemController.ResetAllCommands();
    }

    private void HandleShopOutput(ShopOutputSignal shopOutputSignal)
    {
        shopBehaviorHandler.AnalysisShopBehavior(shopOutputSignal.cards, shopOutputSignal.behaviorType);
    }

    private void ArtifactCommandReceived(ArtifactEffectCommandDispatchSignal artifactEffectCommandDispatchSignal)
    {
        var command = artifactEffectCommandDispatchSignal.command;
        var bUndo = artifactEffectCommandDispatchSignal.bUndo;

        switch (artifactEffectCommandDispatchSignal.type)
        {
            case EffectApplyType.System:
                cardManager.ExecuteCommand(command, bUndo);
                break;
            case EffectApplyType.SlotSystem:
                cardSystemController.GetCardSlotManager().ExecuteCommand(command, bUndo);
                break;
            case EffectApplyType.SelectionSystem:
                cardSelectionManager.ExecuteCommand(command, bUndo);
                break;
            case EffectApplyType.ComplexSystem:
                complexCardEffectResolver.ExecuteCommand(command, bUndo);
                break;
            case EffectApplyType.DataControlSystem:
                cardDataControlManager.ExecuteCommand(command, bUndo);
                break;
        }
    }
}
