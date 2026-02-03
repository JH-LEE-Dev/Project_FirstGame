using UnityEngine;
using System;
using System.Collections.Generic;

public class CardSelectionManager : ICardSelectionSystemActionCommandHandler
{
    public event Action<CardSelectionModeData> CardSelectionStartEvent;
    public delegate void RequestCardSystemActionDelegate(CardLogicSystemActionType type, ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType);
    public RequestCardSystemActionDelegate RequestCardLogicSystemActionEvent;
    public delegate void RequestCardDataControlSystemActionDelegate(CardDataControlSystemActionType type, ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType);
    public RequestCardDataControlSystemActionDelegate RequestCardDataControlSystemActionEvent;

    private CardSelectionMode cardSelectionMode;
    private SelectCardPileType selectCardPileType;

    public void StartCardSelectionMode(SelectCardPileType _selectCardPileType, CardSelectionMode _cardSelectionMode, int amount,List<CardName> _forbiddenCards)
    {
        selectCardPileType = _selectCardPileType;
        cardSelectionMode = _cardSelectionMode;
        CardSelectionModeData data = new CardSelectionModeData(selectCardPileType,cardSelectionMode, amount, _forbiddenCards);

        CardSelectionStartEvent?.Invoke(data);
    }

    public void ExecuteCommand(CardSystemCommand command)
    {
        command.Execute(this);
    }

    public void CardSelectionEnd(CardSelectionModeData _data, List<ICardDataInstanceProvider> _cards)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(_cards.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < _cards.Count; ++i)
        {
            if (_cards[i] != null)
                writeBuffer[i] = _cards[i] as CardDataInstance;
        }

        if (_data.selectionMode == CardSelectionMode.DuplicateCardsToDeck)
            RequestCardLogicSystemActionEvent?.Invoke(CardLogicSystemActionType.DuplicateCardsToDeck, writeBuffer, CardSystemContextType.MAX);
        else if (_data.selectionMode == CardSelectionMode.DuplicateCardsToHand)
            RequestCardLogicSystemActionEvent?.Invoke(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer, CardSystemContextType.MAX);
        else if (_data.selectionMode == CardSelectionMode.UpgradeCardsToHand)
            RequestCardDataControlSystemActionEvent?.Invoke(CardDataControlSystemActionType.CardsUpgraded, writeBuffer, CardSystemContextType.UpgradeCardsFromHand);
        else if(_data.selectionMode == CardSelectionMode.GraveCardsToDeck)
            RequestCardLogicSystemActionEvent?.Invoke(CardLogicSystemActionType.GraveCardsToDeck, writeBuffer, CardSystemContextType.MAX);
        else if(_data.selectionMode == CardSelectionMode.GraveCardsToHand)
            RequestCardLogicSystemActionEvent?.Invoke(CardLogicSystemActionType.GraveCardsToHand, writeBuffer, CardSystemContextType.MAX);
        else if(_data.selectionMode == CardSelectionMode.ExtinctionCardsToDeck)
            RequestCardLogicSystemActionEvent?.Invoke(CardLogicSystemActionType.ExtinctionCardsToDeck, writeBuffer, CardSystemContextType.MAX);
    }
}
