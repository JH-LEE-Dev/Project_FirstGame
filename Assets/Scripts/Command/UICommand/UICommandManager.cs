using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using UICommandSystemSignals;
using CardSystemSignals;

public class UICommandManager : MonoBehaviour
{
    //외부 의존성
    SignalHub signalHub;

    //내부 의존성
    private UICommandDispatcher dispatcher;
    private UICommandFactory_CardSystem commandFactory_CardSystem;

    public void Initialize(SignalHub _signalHub)
    {
        signalHub = _signalHub;

        dispatcher = new UICommandDispatcher();
        commandFactory_CardSystem = new UICommandFactory_CardSystem();

        dispatcher.Initialize(signalHub);
        commandFactory_CardSystem.Initialize();

        SubscribeEvents();
    }

    public void Release()
    {
        UnSubscribeEvents();
    }

    private void SubscribeEvents()
    {
        signalHub.Subscribe<UICommandCompleteEvent>(ReleaseJobBatch);
        signalHub.Subscribe<CardPileDrawEvent,CardDataInstance>(CardPileDrawed);
        signalHub.Subscribe<CardAdditionalDrawEvent,CardDataInstance>(CardAdditionalDrawed);
        signalHub.Subscribe<HandToGraveEvent,CardDataInstance>(HandToGrave);
        signalHub.Subscribe<GraveToDeckEvent,CardDataInstance>(GraveToDeck);
        signalHub.SubscribeScope<CardActionScope>(DispatchCommand);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<UICommandCompleteEvent>(ReleaseJobBatch);
        signalHub.UnSubscribe<CardPileDrawEvent, CardDataInstance>(CardPileDrawed);
        signalHub.UnSubscribe<CardAdditionalDrawEvent, CardDataInstance>(CardAdditionalDrawed);
        signalHub.UnSubscribe<HandToGraveEvent, CardDataInstance>(HandToGrave);
        signalHub.UnSubscribe<GraveToDeckEvent, CardDataInstance>(GraveToDeck);
        signalHub.UnSubscribeScope<CardActionScope>(DispatchCommand);
    }


    public void OnDestroy()
    {
        if (dispatcher != null)
            dispatcher.Release();
    }

    private void CardPileDrawed(CardPileDrawEvent cardPileDrawEvent, ReadOnlySpan<CardDataInstance> cards)
    {
        CreateCommand(ActionType_CardSystem.PileDraw, cards);
    }

    private void CardAdditionalDrawed(CardAdditionalDrawEvent cardAdditionalDrawEvent, ReadOnlySpan<CardDataInstance> cards)
    {
        CreateCommand(ActionType_CardSystem.AdditionalDraw, cards);
    }

    private void HandToGrave(HandToGraveEvent handToGraveEvent, ReadOnlySpan<CardDataInstance> cards)
    {
        CreateCommand(ActionType_CardSystem.HandToGrave, cards);
    }

    private void GraveToDeck(GraveToDeckEvent graveToDeckEvent, ReadOnlySpan<CardDataInstance> cards)
    {
        CreateCommand(ActionType_CardSystem.GraveToDeck, cards);
    }

    public void CreateCommand(ActionType_CardSystem actionType, ReadOnlySpan<CardDataInstance> cards = default)
    {
        switch (actionType)
        {
            case ActionType_CardSystem.PileDraw:
                {
                    commandFactory_CardSystem.CreateJob_Draw(cards, false);
                    break;
                }
            case ActionType_CardSystem.AdditionalDraw:
                {
                    commandFactory_CardSystem.CreateJob_Draw(cards, true);
                    break;
                }
            case ActionType_CardSystem.HandToGrave:
                {
                    commandFactory_CardSystem.CreateJob_ToGrave(cards);
                    break;
                }
            case ActionType_CardSystem.GraveToDeck:
                {
                    commandFactory_CardSystem.CreateJob_ToDeck(cards);
                    break;
                }
        }
    }

    private void DispatchCommand(ScopeSignal<CardActionScope> signal)
    {
        dispatcher.Dispatch_CardSystem(commandFactory_CardSystem.GetJobBatch());
    }

    public void ReleaseJobBatch(UICommandCompleteEvent uiCommandCompleteEvent)
    {
        commandFactory_CardSystem.ReleaseSlot(uiCommandCompleteEvent.commandIdx);
    }
}
