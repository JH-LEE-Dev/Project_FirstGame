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
        signalHub.SubscribeScope<CardActionScopeSignal>(DispatchCommand);
        signalHub.Subscribe<CardSystemEventSignal, CardDataInstance>(ReceiveCardSystemEventSignal);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<UICommandCompleteSignal>(ReleaseJobBatch);
        signalHub.UnSubscribeScope<CardActionScopeSignal>(DispatchCommand);
        signalHub.UnSubscribe<CardSystemEventSignal, CardDataInstance>(ReceiveCardSystemEventSignal);
    }


    public void OnDestroy()
    {
        if (dispatcher != null)
            dispatcher.Release();
    }

    private void ReceiveCardSystemEventSignal(CardSystemEventSignal cardSystemEventSignal,ReadOnlySpan<CardDataInstance> cards = default)
    {
        commandFactory_CardSystem.CreateCommand(cardSystemEventSignal.data, cards);
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
