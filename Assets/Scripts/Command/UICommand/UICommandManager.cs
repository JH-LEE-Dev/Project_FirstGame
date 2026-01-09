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
    }

    public void CreateCommand(JobType_CardSystemUI jobType, ReadOnlySpan<CardDataInstance> cards = default)
    {
        switch (jobType)
        {
            case JobType_CardSystemUI.Draw:
                {
                    commandFactory_CardSystem.CreateJob_Draw(cards);
                    break;
                }
            case JobType_CardSystemUI.HandToGrave:
                {
                    commandFactory_CardSystem.CreateJob_ToGrave(cards);
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
    }

    public void ReleaseDispatchEvent(UIView_HUD HUDObject, UIView_CardSystem cardSystemObject, UIView_Gameplay gameplayObject)
    {
        dispatcher.CardSystem_JobDispatchEvent -= cardSystemObject.RecieveUIJob;
    }
}
