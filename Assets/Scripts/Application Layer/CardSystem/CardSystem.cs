using CardEffectSystemSignal;
using CardSystemSignals;
using GameControlSignals;
using System;
using UnitLogicSystemSignals;

public class CardSystem
{
    private SignalHub signalHub;
    private CardManager cardManager;
    private CardSystemController cardSystemController;

    public void Initialize(SignalHub _signalHub, CardManager _cardManager, CardSystemController _cardSystemController)
    {
        signalHub = _signalHub;
        cardManager = _cardManager;
        cardSystemController = _cardSystemController;

        SubscribeEvents();
        BindEvents();
    }

    private void SubscribeEvents()
    {
        //원래는 CardSystem이 StartCardDrawTurn 정의하여 cardManager Forwarding해야 함. (cardManager 이벤트의 디커플링)
        //하지만 편의성을 위해서 임시적으로 함수를 다이렉트 연결.
        signalHub.Subscribe<PlayerAttackFinishedEvent>(cardSystemController.PlayerAttackFinished);
        signalHub.Subscribe<TryCardUseEvent>(TryCardUse);
        signalHub.Subscribe<PlayerTurnStartEvent>(StartCardDrawTurn);
        signalHub.Subscribe<CardUsingFinishedEvent>(cardSystemController.CardUsingFinished);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<PlayerAttackFinishedEvent>(cardSystemController.PlayerAttackFinished);
        signalHub.UnSubscribe<TryCardUseEvent>(TryCardUse);
        signalHub.UnSubscribe<PlayerTurnStartEvent>(StartCardDrawTurn);
        signalHub.UnSubscribe<CardUsingFinishedEvent>(cardSystemController.CardUsingFinished);
    }

    private void BindEvents()
    {
        cardManager.CardPileDrawEvent -= CardPileDrawed;
        cardManager.CardPileDrawEvent += CardPileDrawed;

        cardManager.CardAdditionalDrawEvent -= CardAdditionalDarwed;
        cardManager.CardAdditionalDrawEvent += CardAdditionalDarwed;

        cardManager.GraveToDeckEvent -= GraveToDeck;
        cardManager.GraveToDeckEvent += GraveToDeck;

        cardManager.HandToGraveEvent -= HandToGrave;
        cardManager.HandToGraveEvent += HandToGrave;

        cardSystemController.CardDrawStartEvent -= CardDrawStarted;
        cardSystemController.CardDrawStartEvent += CardDrawStarted;

        cardSystemController.CardDrawFinishedEvent -= CardDrawFinished;
        cardSystemController.CardDrawFinishedEvent += CardDrawFinished;

        cardSystemController.SystemCommandDispatchEvent -= cardManager.ExecuteCommand;
        cardSystemController.SystemCommandDispatchEvent += cardManager.ExecuteCommand;

        cardSystemController.StatusCommandDispatchEvent -= CardStatusEffectDispatch;
        cardSystemController.StatusCommandDispatchEvent += CardStatusEffectDispatch;

        cardSystemController.CardActionEndScopeEvent -= CardActionEndScope;
        cardSystemController.CardActionEndScopeEvent += CardActionEndScope;

        cardSystemController.PlayerTurnFinishedEvent -= cardManager.PlayerTurnFinished;
        cardSystemController.PlayerTurnFinishedEvent += cardManager.PlayerTurnFinished;
    }

    private void ReleaseEvents()
    {
        cardManager.CardPileDrawEvent -= CardPileDrawed;

        cardManager.CardAdditionalDrawEvent -= CardAdditionalDarwed;

        cardManager.GraveToDeckEvent -= GraveToDeck;

        cardManager.HandToGraveEvent -= HandToGrave;

        cardSystemController.CardDrawStartEvent -= CardDrawStarted;

        cardSystemController.CardDrawFinishedEvent -= CardDrawFinished;

        cardSystemController.SystemCommandDispatchEvent -= cardManager.ExecuteCommand;

        cardSystemController.StatusCommandDispatchEvent -= CardStatusEffectDispatch;

        cardSystemController.CardActionEndScopeEvent -= CardActionEndScope;

        cardSystemController.PlayerTurnFinishedEvent -= cardManager.PlayerTurnFinished;
    }

    public void Release()
    {
        UnSubscribeEvents();
        ReleaseEvents();
    }

    private void CardPileDrawed(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new CardPileDrawEvent(), cards);
    }

    private void CardAdditionalDarwed(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new CardAdditionalDrawEvent(), cards);
    }

    private void GraveToDeck(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new GraveToDeckEvent(), cards);
    }

    private void HandToGrave(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new HandToGraveEvent(), cards);
    }

    private void CardDrawStarted()
    {
        signalHub.Publish(new CardDrawStartEvent());
    }

    private void CardActionEndScope()
    {
        signalHub.EndScope<CardActionScope>(new CardActionScope());
    }

    private void CardDrawFinished()
    {
        signalHub.Publish(new CardDrawFinishedEvent());
    }

    private void CardStatusEffectDispatch(CardSystemCommand command)
    {
        signalHub.Publish(new CardStatusEffectCommandDispatchEvent(command));
    }

    private void StartCardDrawTurn(PlayerTurnStartEvent playerTurnStartEvent)
    {
        cardSystemController.StartCardDrawTurn();
    }

    private void TryCardUse(TryCardUseEvent tryCardUseEvent)
    {
        CardUsedResult result = cardSystemController.TryCardUse(tryCardUseEvent.usedCard);

        if (result.bVerified == true)
            cardManager.CardUsed(result.usedCard);

        signalHub.Publish(new CardUsedEvent(result.bVerified, result.slotIdx));
    }
}
