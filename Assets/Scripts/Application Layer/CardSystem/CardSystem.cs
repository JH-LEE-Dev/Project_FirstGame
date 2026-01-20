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
        signalHub.Subscribe<PlayerAttackFinishedSignal>(cardSystemController.PlayerAttackFinished);
        signalHub.Subscribe<TryCardUseSignal>(TryCardUse);
        signalHub.Subscribe<PlayerTurnStartSignal>(StartCardDrawTurn);
        signalHub.Subscribe<CardUsingFinishedSignal>(cardSystemController.CardUsingFinished);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<PlayerAttackFinishedSignal>(cardSystemController.PlayerAttackFinished);
        signalHub.UnSubscribe<TryCardUseSignal>(TryCardUse);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(StartCardDrawTurn);
        signalHub.UnSubscribe<CardUsingFinishedSignal>(cardSystemController.CardUsingFinished);
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
        signalHub.Publish(new CardPileDrawSignal(), cards);
    }

    private void CardAdditionalDarwed(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new CardAdditionalDrawSignal(), cards);
    }

    private void GraveToDeck(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new GraveToDeckSignal(), cards);
    }

    private void HandToGrave(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new HandToGraveSignal(), cards);
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

        if (result.bVerified == true)
            cardManager.CardUsed(result.usedCard);

        signalHub.Publish(new CardUsedSignal(result.bVerified, result.slotIdx));
    }
}
