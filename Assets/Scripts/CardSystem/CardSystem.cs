using CardEffectSystemSignal;
using CardSystemSignals;
using GameControlSignals;
using System;
using UnityEngine;
using UnitLogicSystemSignals;

public class CardSystem
{
    private SignalHub signalHub;
    private CardManager cardManager;
    private CardEffectCommandManager cardEffectCommandManager;

    public void Initialize(SignalHub _signalHub,CardManager _cardManager,CardEffectCommandManager _cardEffectCommandManager)
    {
        signalHub = _signalHub;
        cardManager = _cardManager;
        cardEffectCommandManager = _cardEffectCommandManager;

        SubscribeEvents();
        BindEvents();
    }

    private void SubscribeEvents()
    {
        //원래는 CardSystem이 StartCardDrawTurn 정의하여 cardManager Forwarding해야 함. (cardManager 이벤트의 디커플링)
        //하지만 편의성을 위해서 임시적으로 함수를 다이렉트 연결.
        signalHub.Subscribe<PlayerTurnStartEvent>(cardManager.StartCardDrawTurn);
        signalHub.Subscribe<PlayerTurnFinishedEvent>(cardManager.PlayerTurnFinished);
        signalHub.Subscribe<CardUsedEvent>(cardManager.CardUsed);
        signalHub.Subscribe<CardUISystemSignals.CardUsingFinishedEvent>(cardManager.CardUsingFinished);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<PlayerTurnStartEvent>(cardManager.StartCardDrawTurn);
        signalHub.UnSubscribe<PlayerTurnFinishedEvent>(cardManager.PlayerTurnFinished);
        signalHub.UnSubscribe<CardUsedEvent>(cardManager.CardUsed);
        signalHub.UnSubscribe<CardUISystemSignals.CardUsingFinishedEvent>(cardManager.CardUsingFinished);
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

        cardManager.CardDrawStartEvent -= CardDrawStarted;
        cardManager.CardDrawStartEvent += CardDrawStarted;

        cardManager.CardActionEndScope -= CardActionEndScope;
        cardManager.CardActionEndScope += CardActionEndScope;

        cardManager.CardDrawFinishedEvent -= CardDrawFinished;
        cardManager.CardActionEndScope += CardDrawFinished;

        cardManager.CardUsingVerificationEvent -= CardUsingVefirication;
        cardManager.CardUsingVerificationEvent += CardUsingVefirication;

        cardManager.CardUsedEvent -= cardEffectCommandManager.AnalysisCardEffect;
        cardManager.CardUsedEvent += cardEffectCommandManager.AnalysisCardEffect;

        cardManager.CardUsingTurnFinishedEvent -= CardUsingTurnFinished;
        cardManager.CardUsingTurnFinishedEvent += CardUsingTurnFinished;

        cardEffectCommandManager.SystemCommandDispatchEvent -= cardManager.InsertCommand;
        cardEffectCommandManager.SystemCommandDispatchEvent += cardManager.InsertCommand;

        cardEffectCommandManager.StatusCommandDispatchEvent -= CardStatusEffectDispatch;
        cardEffectCommandManager.StatusCommandDispatchEvent += CardStatusEffectDispatch;
    }

    private void ReleaseEvents()
    {
        cardManager.CardPileDrawEvent -= CardPileDrawed;

        cardManager.CardAdditionalDrawEvent -= CardAdditionalDarwed;

        cardManager.GraveToDeckEvent -= GraveToDeck;

        cardManager.HandToGraveEvent -= HandToGrave;

        cardManager.CardDrawStartEvent -= CardDrawStarted;

        cardManager.CardActionEndScope -= CardActionEndScope;

        cardManager.CardDrawFinishedEvent -= CardDrawFinished;

        cardManager.CardUsingVerificationEvent -= CardUsingVefirication;

        cardManager.CardUsedEvent -= cardEffectCommandManager.AnalysisCardEffect;

        cardManager.CardUsingTurnFinishedEvent -= CardUsingTurnFinished;

        cardEffectCommandManager.SystemCommandDispatchEvent -= cardManager.InsertCommand;

        cardEffectCommandManager.StatusCommandDispatchEvent -= CardStatusEffectDispatch;
    }

    private void CardPileDrawed(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new CardPileDrawEvent(),cards);
    }

    private void CardAdditionalDarwed(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new CardAdditionalDrawEvent(),cards); 
    }

    private void GraveToDeck(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new  GraveToDeckEvent(),cards);
    }

    private void HandToGrave(ReadOnlySpan<CardDataInstance> cards = default)
    {
        signalHub.Publish(new HandToGraveEvent(),cards);
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

    private void CardUsingVefirication(bool boolean)
    {
        signalHub.Publish(new CardUsingVerificationEvent(boolean));
    }

    private void CardUsingTurnFinished()
    {
        signalHub.Publish(new CardUsingTurnFinishedEvent());
    }

    private void CardStatusEffectDispatch(CardEffectStatusCommand command)
    {
        signalHub.Publish(new CardEffectStatusCommandDispatchEvent(command)); 
    }

    public void Release()
    {
        UnSubscribeEvents();
        ReleaseEvents();
    }
}
