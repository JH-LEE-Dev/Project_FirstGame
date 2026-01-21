using UnityEngine;
using System;
using CardSystemUISignal;
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
        signalHub.Subscribe<UICommandCompleteSignal>(ReleaseJobBatch);
        signalHub.Subscribe<CardPileDrawSignal,CardDataInstance>(CardPileDrawed);
        signalHub.Subscribe<CardAdditionalDrawSignal,CardDataInstance>(CardAdditionalDrawed);
        signalHub.Subscribe<HandToGraveSignal,CardDataInstance>(HandToGrave);
        signalHub.Subscribe<GraveToDeckSignal,CardDataInstance>(GraveToDeck);
        signalHub.SubscribeScope<CardActionScopeSignal>(DispatchCommand);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<UICommandCompleteSignal>(ReleaseJobBatch);
        signalHub.UnSubscribe<CardPileDrawSignal, CardDataInstance>(CardPileDrawed);
        signalHub.UnSubscribe<CardAdditionalDrawSignal, CardDataInstance>(CardAdditionalDrawed);
        signalHub.UnSubscribe<HandToGraveSignal, CardDataInstance>(HandToGrave);
        signalHub.UnSubscribe<GraveToDeckSignal, CardDataInstance>(GraveToDeck);
        signalHub.UnSubscribeScope<CardActionScopeSignal>(DispatchCommand);
    }


    public void OnDestroy()
    {
        if (dispatcher != null)
            dispatcher.Release();
    }

    private void CardPileDrawed(CardPileDrawSignal cardPileDrawSignal, ReadOnlySpan<CardDataInstance> cards)
    {
        CreateCommand(ActionType_CardSystem.PileDraw, cards);
    }

    private void CardAdditionalDrawed(CardAdditionalDrawSignal cardAdditionalDrawSignal, ReadOnlySpan<CardDataInstance> cards)
    {
        CreateCommand(ActionType_CardSystem.AdditionalDraw, cards);
    }

    private void HandToGrave(HandToGraveSignal handToGraveSignal, ReadOnlySpan<CardDataInstance> cards)
    {
        CreateCommand(ActionType_CardSystem.HandToGrave, cards);
    }

    private void GraveToDeck(GraveToDeckSignal graveToDeckSignal, ReadOnlySpan<CardDataInstance> cards)
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

    private void DispatchCommand(ScopeSignal<CardActionScopeSignal> signal)
    {
        dispatcher.Dispatch_CardSystem(commandFactory_CardSystem.GetJobBatch());
    }

    public void ReleaseJobBatch(UICommandCompleteSignal uiCommandCompleteSignal)
    {
        commandFactory_CardSystem.ReleaseSlot(uiCommandCompleteSignal.commandIdx);
    }
}
