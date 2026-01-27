using UnityEngine;
using System;
using System.Collections.Generic;

public class CardSelectionManager : ICardSelectionSystemActionCommandHandler
{
    public event Action<CardSelectionModeData> CardSelectionStartEvent;
    public delegate void RequestCardSystemActionDelegate(CardSystemActionType type, ReadOnlySpan<CardDataInstance> cards);
    public RequestCardSystemActionDelegate RequestCardSystemActionEvent;

    private CardSelectionMode cardSelectionMode;

    public void StartCardSelectionMode(CardSelectionMode _cardSelectionMode, int amount)
    {
        cardSelectionMode = _cardSelectionMode;
        CardSelectionModeData data = new CardSelectionModeData(cardSelectionMode, amount);

        CardSelectionStartEvent?.Invoke(data);
    }

    public void ExecuteCommand(CardSystemCommand command)
    {
        command.Execute(this);
    }

    public void CardSelectionEnd(CardSelectionModeData _data, List<CardDataInstance> _cards)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(_cards.Count);

        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < _cards.Count; ++i)
        {
            if (_cards[i] != null)
                writeBuffer[i] = _cards[i];
        }

        if (_data.selectionMode == CardSelectionMode.DuplicateToDeck)
            RequestCardSystemActionEvent?.Invoke(CardSystemActionType.DuplicateCardsToDeck, writeBuffer);
        else if (_data.selectionMode == CardSelectionMode.DuplicateToHand)
            RequestCardSystemActionEvent?.Invoke(CardSystemActionType.DuplicateCardsToHand, writeBuffer);
        else if (_data.selectionMode == CardSelectionMode.UpgradeToHand)
            UpgradeCards(_cards);

        rentalBuffer.Dispose();
    }

    public void UpgradeCards(List<CardDataInstance> _cards)
    {
        for (int i = 0; i < _cards.Count; ++i)
        {
            if (_cards[i] != null)
                _cards[i].bUpgrade = true;
        }
    }
}
