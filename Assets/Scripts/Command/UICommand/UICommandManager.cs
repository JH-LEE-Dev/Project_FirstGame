using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using UICommandSystemSignals;
using CardSystemSignals;

public class UICommandManager : MonoBehaviour
{
    //외부 의존성
    ISignalHub<IPulicSignal> publicSignalHub;
    ISignalHub<ICardSystemPrivateSignal> cardSystemSignalHub;

    //내부 의존성
    private UICommandDispatcher dispatcher;
    private UICommandFactory_CardSystem commandFactory_CardSystem;

    public void Initialize(ISignalHub<IPulicSignal> _publicSignalHub,ISignalHub<ICardSystemPrivateSignal> _cardSystemSignalHub)
    {
        publicSignalHub = _publicSignalHub;
        cardSystemSignalHub = _cardSystemSignalHub;

        dispatcher = new UICommandDispatcher();
        commandFactory_CardSystem = new UICommandFactory_CardSystem();

        dispatcher.Initialize(publicSignalHub);
        commandFactory_CardSystem.Initialize();

        SubscribeEvents();
    }

    public void Release()
    {
        UnSubscribeEvents();
    }

    private void SubscribeEvents()
    {
        publicSignalHub.Subscribe<UICommandCompleteEvent>(ReleaseJobBatch);
        cardSystemSignalHub.Subscribe<CardPileDrawEvent,CardDataInstance>(CardPileDrawed);
        cardSystemSignalHub.Subscribe<CardAdditionalDrawEvent,CardDataInstance>(CardAdditionalDrawed);
        cardSystemSignalHub.Subscribe<HandToGraveEvent,CardDataInstance>(HandToGrave);
        cardSystemSignalHub.Subscribe<GraveToDeckEvent,CardDataInstance>(GraveToDeck);
        cardSystemSignalHub.SubscribeScope<CardActionScope>(DispatchCommand);
    }

    private void UnSubscribeEvents()
    {
        publicSignalHub.UnSubscribe<UICommandCompleteEvent>(ReleaseJobBatch);
        cardSystemSignalHub.UnSubscribe<CardPileDrawEvent, CardDataInstance>(CardPileDrawed);
        cardSystemSignalHub.UnSubscribe<CardAdditionalDrawEvent, CardDataInstance>(CardAdditionalDrawed);
        cardSystemSignalHub.UnSubscribe<HandToGraveEvent, CardDataInstance>(HandToGrave);
        cardSystemSignalHub.UnSubscribe<GraveToDeckEvent, CardDataInstance>(GraveToDeck);
        cardSystemSignalHub.UnSubscribeScope<CardActionScope>(DispatchCommand);
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
