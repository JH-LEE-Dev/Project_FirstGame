using CardSystemSignals;
using GameControlSignals;
using UICommandSystemSignals;
using UnityEngine;
using CardSystemUISignal;
using System;

public class CardUICoordinator
{
    public event Action<int,CardDataInstance> CardEquippedEvent;
    public event Action<CardDataInstance> TryCardUseEvent;
    public event Action CardUsingFinishedEvent;
    public event Action<int> UICommandCompleteEvent;

    private UIView_CardSystem cardUISystem;

    public void Initialize(UIView_CardSystem _cardUISystem)
    {
        cardUISystem = _cardUISystem;

        BindEvent();
    }

    public void Release()
    {
        ReleaseEvent();
    }

    private void UICommandComplete(int idx)
    {
        UICommandCompleteEvent?.Invoke(idx);
    }

    private void BindEvent()
    {
        cardUISystem.TryCardUseEvent -= TryCardUse;
        cardUISystem.TryCardUseEvent += TryCardUse;

        cardUISystem.CardUsingFinishedEvent -= CardUsingFinished;
        cardUISystem.CardUsingFinishedEvent += CardUsingFinished;

        cardUISystem.UICommandCompleteEvent -= UICommandComplete;
        cardUISystem.UICommandCompleteEvent += UICommandComplete;

        cardUISystem.CardEquippedEvent -= CardEquipped;
        cardUISystem.CardEquippedEvent += CardEquipped;
    }

    private void ReleaseEvent()
    {
        cardUISystem.UICommandCompleteEvent -= UICommandComplete;

        cardUISystem.TryCardUseEvent -= TryCardUse;

        cardUISystem.CardUsingFinishedEvent -= CardUsingFinished;

        cardUISystem.CardEquippedEvent -= CardEquipped;
    }

    public void TryCardUse(CardDataInstance usedCard)
    {
        TryCardUseEvent?.Invoke(usedCard);
    }

    public void CardDrawFinished()
    {
        cardUISystem.CardDrawFinished();
    }

    public void CardUsingFinished()
    {
        CardUsingFinishedEvent?.Invoke();
    }

    public void CardUsed(bool bVerified,int slotIdx,Transform transform)
    {
        cardUISystem.CardUsingApproved(bVerified, slotIdx, transform);
    }

    public void RecieveUIJob(CardUIActionBatch actionDataBatch)
    {
        cardUISystem.ReceiveUIAction(actionDataBatch);
    }

    public void EnemyTurnStarted()
    {
        cardUISystem.EnemyTurnStarted();
    }

    public void PlayerTurnStarted()
    {
        cardUISystem.PlayerTurnStarted();
    }

    private void CardEquipped(int slotIdx,CardDataInstance equippedCard)
    {
        CardEquippedEvent?.Invoke(slotIdx, equippedCard);   
    }

    public void UnEquipBulletCard(int idx)
    {
       cardUISystem.UnEquipBulletCard(idx);
    }

    public void CancelPreview()
    {
        cardUISystem.CancelPreview();
    }

    public void CardSelectionModeStarted(CardSelectionModeData data)
    {
        cardUISystem.StartCardSelectMode(data.amount, true);
    }
}
