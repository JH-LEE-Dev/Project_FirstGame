using UnityEngine;
using System;
using System.Collections.Generic;

public class CardUICoordinator
{
    public event Action<int, ICardDataInstanceProvider> CardEquippedEvent;
    public event Action<ICardDataInstanceProvider> TryCardUseEvent;
    public event Action CardUsingFinishedEvent;
    public event Action<int> UICommandCompleteEvent;
    public event Action<CardSelectionModeData, List<ICardDataInstanceProvider>> CardSelectionEndEvent;

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

        cardUISystem.CardSelectionEndEvent -= CardSelectionEnd;
        cardUISystem.CardSelectionEndEvent += CardSelectionEnd;
    }

    private void ReleaseEvent()
    {
        cardUISystem.UICommandCompleteEvent -= UICommandComplete;

        cardUISystem.TryCardUseEvent -= TryCardUse;

        cardUISystem.CardUsingFinishedEvent -= CardUsingFinished;

        cardUISystem.CardEquippedEvent -= CardEquipped;

        cardUISystem.CardSelectionEndEvent -= CardSelectionEnd;
    }

    public void TryCardUse(ICardDataInstanceProvider usedCard)
    {
        TryCardUseEvent?.Invoke(usedCard);
    }

    public void CardUsePhaseStarted()
    {
        cardUISystem.CardUsePhaseStarted();
    }

    public void CardUsingFinished()
    {
        CardUsingFinishedEvent?.Invoke();
    }

    public void CardUsed(bool bVerified, int slotIdx, Transform transform)
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

    private void CardEquipped(int slotIdx, ICardDataInstanceProvider equippedCard)
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
        cardUISystem.StartCardSelectMode(data, data.amount, true);
    }

    private void CardSelectionEnd(List<ICardDataInstanceProvider> _cards,CardSelectionModeData data)
    {
        CardSelectionEndEvent?.Invoke(data, _cards);
    }

    public void ShopTimeStarted()
    {

    }
}
