using CardEffectSystemSignal;
using CardSystemSignals;
using GameControlSignals;
using System;
using UnitLogicSystemSignals;
using CardSystemUISignal;

public class CardSystem
{
    private SignalHub signalHub;
    private CardManager cardManager;
    private CardSystemController cardSystemController;
    private ComplexCardEffectResolver complexCardEffectResolver;

    public void Initialize(SignalHub _signalHub, CardManager _cardManager,
        CardSystemController _cardSystemController, ComplexCardEffectResolver _complexCardEffectResolver)
    {
        signalHub = _signalHub;
        cardManager = _cardManager;
        cardSystemController = _cardSystemController;
        complexCardEffectResolver = _complexCardEffectResolver;

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
        signalHub.Subscribe<DiscardBulletCardSignal>(DiscardBulletCard);
        signalHub.Subscribe<PlayerAttackedSignal>(PlayerAttacked);
        signalHub.Subscribe<WaveStartSignal>(WaveStarted);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<PlayerAttackFinishedSignal>(cardSystemController.PlayerAttackFinished);
        signalHub.UnSubscribe<TryCardUseSignal>(TryCardUse);
        signalHub.UnSubscribe<PlayerTurnStartSignal>(StartCardDrawTurn);
        signalHub.UnSubscribe<CardUsingFinishedSignal>(cardSystemController.CardUsingFinished);
        signalHub.UnSubscribe<DiscardBulletCardSignal>(DiscardBulletCard);
        signalHub.UnSubscribe<PlayerAttackedSignal>(PlayerAttacked);
        signalHub.UnSubscribe<WaveStartSignal>(WaveStarted);
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

        cardManager.CardToExtinctionEvent -= UsedCardToExtinction;
        cardManager.CardToExtinctionEvent += UsedCardToExtinction;

        cardManager.CardToGraveEvent -= UsedCardToGrave;
        cardManager.CardToGraveEvent += UsedCardToGrave;

        cardManager.ExtinctionToDeckEvent -= ExtinctionToDeck;
        cardManager.ExtinctionToDeckEvent += ExtinctionToDeck;

        cardManager.GraveToHandEvent -= GraveToHand;
        cardManager.GraveToHandEvent += GraveToHand;

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

        cardSystemController.UsedCardRemovedFromHandEvent -= cardManager.CardRemoveFromHand;
        cardSystemController.UsedCardRemovedFromHandEvent += cardManager.CardRemoveFromHand;

        cardSystemController.UsedCardToExtinctionEvent -= cardManager.ExtinguishCards;
        cardSystemController.UsedCardToExtinctionEvent += cardManager.ExtinguishCards;

        cardSystemController.CardsToExtinctionEvent -= cardManager.CardsToExtinction;
        cardSystemController.CardsToExtinctionEvent += cardManager.CardsToExtinction;

        cardSystemController.CardsRemoveFronHandsEvent -= cardManager.CardRemoveFromHand;
        cardSystemController.CardsRemoveFronHandsEvent += cardManager.CardRemoveFromHand;

        cardSystemController.UsedCardToGraveEvent -= cardManager.CardToGrave;
        cardSystemController.UsedCardToGraveEvent += cardManager.CardToGrave;

        cardSystemController.ExtinctionToDeckEvent -= cardManager.ExtinctionToDeck;
        cardSystemController.ExtinctionToDeckEvent += cardManager.ExtinctionToDeck;

        cardSystemController.ComplexCommandDispatchEvent -= complexCardEffectResolver.ExecuteCommand;
        cardSystemController.ComplexCommandDispatchEvent += complexCardEffectResolver.ExecuteCommand;

        cardSystemController.EquippedCardsToExtinctionEvent -= EquippedCardToExtinction;
        cardSystemController.EquippedCardsToExtinctionEvent += EquippedCardToExtinction;

        cardSystemController.EquippedCardsToGraveEvent -= EquippedCardToGrave;
        cardSystemController.EquippedCardsToGraveEvent += EquippedCardToGrave;

        cardSystemController.CardSlotCntChangedEvent -= CardSlotCntChanged;
        cardSystemController.CardSlotCntChangedEvent += CardSlotCntChanged;
    }

    private void ReleaseEvents()
    {
        cardManager.CardPileDrawEvent -= CardPileDrawed;

        cardManager.CardAdditionalDrawEvent -= CardAdditionalDarwed;

        cardManager.GraveToDeckEvent -= GraveToDeck;

        cardManager.HandToGraveEvent -= HandToGrave;

        cardManager.CardToExtinctionEvent -= UsedCardToExtinction;

        cardManager.ExtinctionToDeckEvent -= ExtinctionToDeck;

        cardManager.GraveToHandEvent -= GraveToHand;

        cardSystemController.CardDrawStartEvent -= CardDrawStarted;

        cardSystemController.CardDrawFinishedEvent -= CardDrawFinished;

        cardSystemController.SystemCommandDispatchEvent -= cardManager.ExecuteCommand;

        cardSystemController.StatusCommandDispatchEvent -= CardStatusEffectDispatch;

        cardSystemController.CardActionEndScopeEvent -= CardActionEndScope;

        cardSystemController.PlayerTurnFinishedEvent -= cardManager.PlayerTurnFinished;

        cardSystemController.UsedCardRemovedFromHandEvent -= cardManager.CardRemoveFromHand;

        cardSystemController.UsedCardToExtinctionEvent -= cardManager.ExtinguishCards;

        cardSystemController.UsedCardToGraveEvent -= cardManager.CardToGrave;

        cardSystemController.ExtinctionToDeckEvent -= cardManager.ExtinctionToDeck;

        cardSystemController.ComplexCommandDispatchEvent -= complexCardEffectResolver.ExecuteCommand;

        cardSystemController.CardsToExtinctionEvent -= cardManager.CardsToExtinction;

        cardSystemController.CardsRemoveFronHandsEvent -= cardManager.CardRemoveFromHand;

        cardSystemController.EquippedCardsToExtinctionEvent -= EquippedCardToExtinction;

        cardSystemController.EquippedCardsToGraveEvent -= EquippedCardToGrave;

        cardSystemController.CardSlotCntChangedEvent -= CardSlotCntChanged;
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

    private void UsedCardToExtinction(ReadOnlySpan<CardDataInstance> cardPile)
    {
        signalHub.Publish(new UsedCardToExtinctionSignal(), cardPile);
    }

    private void UsedCardToGrave(ReadOnlySpan<CardDataInstance> cardPile)
    {
        signalHub.Publish(new UsedCardToGraveSignal(), cardPile);
    }

    private void ExtinctionToDeck(ReadOnlySpan<CardDataInstance> cardPile)
    {
        signalHub.Publish(new ExtinctionToDeckSignal(), cardPile);
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

    private void GraveToHand(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new GraveToHandSignal(), cards);
    }

    private void EquippedCardToExtinction(ReadOnlySpan<CardDataInstance> cards = default)
    {
        //signalHub.Publish(new EquippedCardToExtinctionSignal(), cards);
    }

    private void EquippedCardToGrave(ReadOnlySpan<CardDataInstance> cards = default)
    {
        //signalHub.Publish(new EquippedCardToGraveSignal(), cards);
    }

    private void CardSlotCntChanged(int cnt)
    {
        signalHub.Publish(new CardSlotCntChangedSignal(cnt));
    }
}
