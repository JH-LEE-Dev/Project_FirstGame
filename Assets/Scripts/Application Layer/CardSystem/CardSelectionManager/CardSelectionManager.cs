using UnityEngine;
using System;
using System.Collections.Generic;

public class CardSelectionManager : ICardSelectionSystemActionCommandHandler
{
    public event Action<CardSelectionModeData> CardSelectionStartEvent;
    public delegate void RequestCardSystemActionDelegate(CardSystemActionType type, ReadOnlySpan<CardDataInstance> cards);
    public RequestCardSystemActionDelegate RequestCardSystemActionEvent;

    private CardSelectionMode cardSelectionMode;
    private SelectCardPileType selectCardPileType;

    public void StartCardSelectionMode(SelectCardPileType _selectCardPileType, CardSelectionMode _cardSelectionMode, int amount)
    {
        selectCardPileType = _selectCardPileType;
        cardSelectionMode = _cardSelectionMode;
        CardSelectionModeData data = new CardSelectionModeData(selectCardPileType,cardSelectionMode, amount);

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

        if (_data.selectionMode == CardSelectionMode.DuplicateCardsToDeck)
            RequestCardSystemActionEvent?.Invoke(CardSystemActionType.DuplicateCardsToDeck, writeBuffer);
        else if (_data.selectionMode == CardSelectionMode.DuplicateCardsToHand)
            RequestCardSystemActionEvent?.Invoke(CardSystemActionType.DuplicateCardsToHand, writeBuffer);
        else if (_data.selectionMode == CardSelectionMode.UpgradeCardsToHand)
            UpgradeCards(_cards);
        else if(_data.selectionMode == CardSelectionMode.GraveCardsToDeck)
            RequestCardSystemActionEvent?.Invoke(CardSystemActionType.GraveCardsToDeck, writeBuffer);
        else if(_data.selectionMode == CardSelectionMode.GraveCardsToHand)
            RequestCardSystemActionEvent?.Invoke(CardSystemActionType.GraveCardsToHand, writeBuffer);

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
