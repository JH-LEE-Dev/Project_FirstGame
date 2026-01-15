using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using UICommandSystemSignals;

public class UICommandManager : MonoBehaviour, ICardUICommandSystem
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
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<UICommandCompleteEvent>(ReleaseJobBatch);
    }


    public void OnDestroy()
    {
        if (dispatcher != null)
            dispatcher.Release();
    }

    public void CreateCommand(JobType_CardSystemUI jobType, ReadOnlySpan<CardDataInstance> cards = default)
    {
        switch (jobType)
        {
            case JobType_CardSystemUI.Draw:
                {
                    commandFactory_CardSystem.CreateJob_Draw(cards, false);
                    break;
                }
            case JobType_CardSystemUI.AdditionalDraw:
                {
                    commandFactory_CardSystem.CreateJob_Draw(cards, true);
                    break;
                }
            case JobType_CardSystemUI.HandToGrave:
                {
                    commandFactory_CardSystem.CreateJob_ToGrave(cards);
                    break;
                }
            case JobType_CardSystemUI.GraveToDeck:
                {
                    commandFactory_CardSystem.CreateJob_ToDeck(cards);
                    break;
                }
        }
    }

    public void DispatchCommand()
    {
        dispatcher.Dispatch_CardSystem(commandFactory_CardSystem.GetJobBatch());
    }

    public void ReleaseJobBatch(UICommandCompleteEvent uiCommandCompleteEvent)
    {
        commandFactory_CardSystem.ReleaseSlot(uiCommandCompleteEvent.commandIdx);
    }
}
