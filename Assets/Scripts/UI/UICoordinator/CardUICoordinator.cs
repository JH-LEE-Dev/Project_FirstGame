using CardSystemSignals;
using CardUISystemSignals;
using GameControlSignals;
using System;
using System.Collections.Generic;
using UICommandSystemSignals;
using UnitSpawnSystemSignals;
using UnityEngine;

public class CardUICoordinator
{
    //외부 의존성
    SignalHub signalHub;


    private UIView_CardSystem cardUISystem;
    private UIView_Unit unitUISystem;

    public void Initialize(SignalHub _signalHub,UIView_CardSystem _cardUISystem,UIView_Unit _unitUISystem)
    {
        signalHub = _signalHub;
        cardUISystem = _cardUISystem;
        unitUISystem = _unitUISystem;

        BindEvent();

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        signalHub.Subscribe<EnemyTurnStartEvent>(EnemyTurnStarted);
        signalHub.Subscribe<PlayerTurnStartEvent>(PlayerTurnStarted);
        signalHub.Subscribe<CardDrawFinishedEvent>(CardDrawFinished);
        signalHub.Subscribe<CardUsingVerificationEvent>(CardUsingApproved);
        signalHub.Subscribe<CharacterSpawnedEvent>(CharacterSpawned);
        signalHub.Subscribe<CardSystem_JobDispatchEvent>(RecieveUIJob);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<EnemyTurnStartEvent>(EnemyTurnStarted);
        signalHub.UnSubscribe<PlayerTurnStartEvent>(PlayerTurnStarted);
        signalHub.UnSubscribe<CardDrawFinishedEvent>(CardDrawFinished);
        signalHub.UnSubscribe<CardUsingVerificationEvent>(CardUsingApproved);
        signalHub.UnSubscribe<CharacterSpawnedEvent>(CharacterSpawned);
        signalHub.UnSubscribe<CardSystem_JobDispatchEvent>(RecieveUIJob);
    }

    public void Release()
    {
        UnSubscribeEvents();
        ReleaseEvent();
    }

    private void UICommandComplete(int idx)
    {
        signalHub.Publish(new  UICommandCompleteEvent(idx));
    }

    private void BindEvent()
    {
        cardUISystem.CardUsedEvent -= CardUsed;
        cardUISystem.CardUsedEvent += CardUsed;
        cardUISystem.CardUsingFinishedEvent -= CardUsingFinished;
        cardUISystem.CardUsingFinishedEvent += CardUsingFinished;
        cardUISystem.UICommandCompleteEvent -= UICommandComplete;
        cardUISystem.UICommandCompleteEvent += UICommandComplete;
    }

    public void CharacterSpawned(CharacterSpawnedEvent characterSpawnedEvent)
    {
        unitUISystem.Initialize(characterSpawnedEvent.characterData);
    }

    private void ReleaseEvent()
    {
        cardUISystem.UICommandCompleteEvent -= UICommandComplete;
        cardUISystem.CardUsedEvent -= CardUsed;
        cardUISystem.CardUsingFinishedEvent -= CardUsingFinished;
    }

    public void CardUsed(CardDataInstance usedCard)
    {
        signalHub.Publish(new CardUISystemSignals.CardUsedEvent(usedCard));
    }

    public void CardDrawFinished(CardDrawFinishedEvent cardDrawFinishedEvent)
    {
        cardUISystem.CardDrawFinished();
    }

    public void CardUsingFinished()
    {
        signalHub.Publish(new CardUISystemSignals.CardUsingFinishedEvent());
    }

    public void CardUsingApproved(CardUsingVerificationEvent cardUsingVerificationEvent)
    {
        cardUISystem.CardUsingApproved(cardUsingVerificationEvent.bVerified);
    }

    public void RecieveUIJob(CardSystem_JobDispatchEvent cardSystem_JobDispatchEvent)
    {
        cardUISystem.RecieveUIJob(cardSystem_JobDispatchEvent.jobBatch);
    }

    public void EnemyTurnStarted(EnemyTurnStartEvent enemyTurnStartEvent)
    {
        cardUISystem.EnemyTurnStarted();
    }

    public void PlayerTurnStarted(PlayerTurnStartEvent playerTurnStartEvent)
    {
        cardUISystem.PlayerTurnStarted();
    }
}
