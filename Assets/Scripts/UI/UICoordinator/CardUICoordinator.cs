using System;
using System.Collections.Generic;
using UnityEngine;

public class CardUICoordinator
{
    public event Action UICommandCompleteEvent;
    public event Action<CardDataInstance> CardUsedEvent;
    public event Action CardUsingFinishedEvent;

    private UIView_CardSystem cardUISystem;
    private UIView_Unit unitUISystem;

    public void Initialize(UIView_CardSystem _cardUISystem,UIView_Unit _unitUISystem)
    {
        cardUISystem = _cardUISystem;
        unitUISystem = _unitUISystem;

        BindEvent();
    }

    public void Release()
    {
        ReleaseEvent();
    }

    private void UICommandComplete()
    {
        UICommandCompleteEvent?.Invoke();
    }

    private void BindEvent()
    {
        cardUISystem.CardUsedEvent -= CardUsed;
        cardUISystem.CardUsedEvent += CardUsed;
        cardUISystem.CardUsingFinishedEvent -= CardUsingFinished;
        cardUISystem.CardUsingFinishedEvent += CardUsingFinished;
    }

    private void ReleaseEvent()
    {
        cardUISystem.CardUsedEvent -= CardUsed;
        cardUISystem.CardUsingFinishedEvent -= CardUsingFinished;
    }

    public void CardUsed(CardDataInstance usedCard)
    {
        CardUsedEvent?.Invoke(usedCard);
    }

    public void CardDrawFinished()
    {
        cardUISystem.CardDrawFinished();
    }

    public void CardUsingFinished()
    {
        CardUsingFinishedEvent?.Invoke();
    }

    public void CardUsingApproved(bool boolean)
    {
        cardUISystem.CardUsingApproved(boolean);
    }

    public void RecieveUIJob(List<Job_CardSystemUI> _jobQueue)
    {
        cardUISystem.RecieveUIJob(_jobQueue);
    }

    public void EnemyTurnStarted()
    {
        cardUISystem.EnemyTurnStarted();
    }

    public void PlayerTurnStarted(int waveIdx)
    {
        cardUISystem.PlayerTurnStarted(waveIdx);
    }
}
