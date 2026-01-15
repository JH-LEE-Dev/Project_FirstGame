using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class UICommandManager : MonoBehaviour, ICardUICommandSystem,ICardUICommandEvents
{
    // 인터페이스의 이벤트를 dispatcher에 직접 연결
    public event Action<UIJobBatch_CardSystem> JobDispatchEvent
    {
        add => dispatcher.CardSystem_JobDispatchEvent += value;
        remove => dispatcher.CardSystem_JobDispatchEvent -= value;
    }

    private UICommandDispatcher dispatcher;
    private UICommandFactory_CardSystem commandFactory_CardSystem;

    public void Initialize()
    {
        dispatcher = new UICommandDispatcher();
        commandFactory_CardSystem = new UICommandFactory_CardSystem();

        commandFactory_CardSystem.Initialize();
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

    public void ReleaseJobBatch(int idx)
    {
        commandFactory_CardSystem.ReleaseSlot(idx);
    }
}
