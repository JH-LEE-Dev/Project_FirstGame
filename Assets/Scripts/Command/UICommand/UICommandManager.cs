using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class UICommandManager : MonoBehaviour, ICardUICommandSystem
{
    private UICommandDispatcher dispatcher;
    private UICommandFactory_CardSystem commandFactory_CardSystem;

    public void Initialize()
    {
        dispatcher = new UICommandDispatcher();
        commandFactory_CardSystem = new UICommandFactory_CardSystem();

        commandFactory_CardSystem.Initialize();
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

    public void BindDispatchEvent(UIView_HUD HUDObject, UIView_CardSystem cardSystemObject, UIView_Gameplay gameplayObject)
    {
        dispatcher.CardSystem_JobDispatchEvent -= cardSystemObject.RecieveUIJob;
        dispatcher.CardSystem_JobDispatchEvent += cardSystemObject.RecieveUIJob;
        cardSystemObject.UICommandCompleteEvent -= commandFactory_CardSystem.DecreaseBatchCount;
        cardSystemObject.UICommandCompleteEvent += commandFactory_CardSystem.DecreaseBatchCount;
    }

    public void ReleaseDispatchEvent(UIView_HUD HUDObject, UIView_CardSystem cardSystemObject, UIView_Gameplay gameplayObject)
    {
        if (cardSystemObject != null)
        {
            dispatcher.CardSystem_JobDispatchEvent -= cardSystemObject.RecieveUIJob;
            cardSystemObject.UICommandCompleteEvent -= commandFactory_CardSystem.DecreaseBatchCount;
        }
    }
}
